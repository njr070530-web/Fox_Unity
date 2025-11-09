import threading
import socket
import json
import time
import argparse
import cv2
import mediapipe as mp
import pyaudio
import numpy as np

parser = argparse.ArgumentParser()
parser.add_argument("--ip", default="127.0.0.1")
parser.add_argument("--port", type=int, default=5005)
parser.add_argument("--camera", type=int, default=0)
parser.add_argument("--resize", type=int, default=640)
args = parser.parse_args()

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
addr = (args.ip, args.port)

# ----------- 音频捕获线程 -----------
latest_voice = {"volume": 0.0, "pitch": 0.5, "isSpeech": False, "timestamp": 0.0}

def run_voice():
    global latest_voice
    CHUNK = 1024
    FORMAT = pyaudio.paInt16
    CHANNELS = 1
    RATE = 44100
    p = pyaudio.PyAudio()
    stream = p.open(format=FORMAT, channels=CHANNELS, rate=RATE, input=True, frames_per_buffer=CHUNK)
    try:
        while True:
            data = np.frombuffer(stream.read(CHUNK, exception_on_overflow=False), dtype=np.int16)
            volume = np.linalg.norm(data) / (CHUNK * 32768)
            raw_norm = np.linalg.norm(data) / CHUNK
            volume_voice = raw_norm / 5.0
            # pitch = np.argmax(np.abs(np.fft.rfft(data))) / len(data)
            is_speech = volume_voice > 30.0
            fft_data = np.fft.rfft(data)
            freqs = np.fft.rfftfreq(len(data), d=1.0/RATE)
            pitch_hz = freqs[np.argmax(np.abs(fft_data))]

            # is_speech = volume > 0.05
            latest_voice = {
                "volume": float(volume_voice),
                "pitch": float(pitch_hz),
                "isSpeech": bool(is_speech),
                "timestamp": time.time()
            }
            print(f"[DEBUG] volume={volume_voice:.3f}, pitch={pitch_hz:.1f}Hz, max={np.max(data)}, min={np.min(data)}")
    finally:
        stream.stop_stream()
        stream.close()
        p.terminate()

# ----------- 姿势捕获主线程 -----------
def run_pose():
    mp_pose = mp.solutions.pose
    pose = mp_pose.Pose(static_image_mode=False,
                        model_complexity=1,
                        enable_segmentation=False,
                        min_detection_confidence=0.5,
                        min_tracking_confidence=0.5)
    LANDMARK_NAMES = {lm.value: lm.name for lm in mp_pose.PoseLandmark}

    cap = cv2.VideoCapture(args.camera)
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, args.resize)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, int(args.resize * 3 / 4))

    try:
        while True:
            ret, frame = cap.read()
            if not ret:
                break

            h, w = frame.shape[:2]
            image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            results = pose.process(image_rgb)

            packet = {
                "timestamp": time.time(),
                "landmarks": None,
                "width": w,
                "height": h,
                "voice": latest_voice,  # ✅ 合并语音数据
            }

            if results.pose_landmarks:
                lm = results.pose_landmarks.landmark
                landmarks = {
                    LANDMARK_NAMES[i]: [float(lm[i].x), float(lm[i].y), float(lm[i].z), float(lm[i].visibility)]
                    for i in range(len(lm))
                }
                packet["landmarks"] = landmarks

            sock.sendto(json.dumps(packet).encode("utf-8"), addr)

            if results.pose_landmarks:
                mp.solutions.drawing_utils.draw_landmarks(frame, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)
            cv2.imshow("Pose + Voice", frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

    finally:
        cap.release()
        cv2.destroyAllWindows()
        pose.close()
        sock.close()

# 启动
t_voice = threading.Thread(target=run_voice, daemon=True)
t_voice.start()
run_pose()


