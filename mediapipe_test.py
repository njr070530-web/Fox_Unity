import threading
import socket
import json
import time
import argparse
import cv2
import mediapipe as mp
import pyaudio
import numpy as np

# ----------- 可选固定麦克风编号（你想手动改可以写这里）-----------
MIC_INDEX = None   # 写 None = 自动选择可用麦克风

parser = argparse.ArgumentParser()
parser.add_argument("--ip", default="127.0.0.1")
parser.add_argument("--port", type=int, default=5005)
parser.add_argument("--video_port", type=int, default=5007)
parser.add_argument("--camera", type=int, default=0)
parser.add_argument("--resize", type=int, default=640)
args = parser.parse_args()

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
addr = (args.ip, args.port)

sock_video = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
video_addr = (args.ip, args.video_port)

# ----------- 音频捕获线程 -----------
latest_voice = {"volume": 0.0, "pitch": 0.5, "isSpeech": False, "timestamp": 0.0}

def run_voice():
    global latest_voice

    CHUNK = 1024
    FORMAT = pyaudio.paInt16
    CHANNELS = 1
    RATE = 44100

    p = pyaudio.PyAudio()

    # ---- 自动选择麦克风 ----
    device_index = MIC_INDEX
    if device_index is None:
        for i in range(p.get_device_count()):
            info = p.get_device_info_by_index(i)
            if info["maxInputChannels"] > 0:
                device_index = i
                print(f"Using mic: index={i}, name={info['name']}")
                break

    if device_index is None:
        print("No input device found!")
        return

    try:
        stream = p.open(
            format=FORMAT,
            channels=CHANNELS,
            rate=RATE,
            input=True,
            input_device_index=device_index,
            frames_per_buffer=CHUNK
        )
    except Exception as e:
        print("Mic open failed:", e)
        return

    print("Mic started.")

    try:
        while True:
            data = np.frombuffer(stream.read(CHUNK, exception_on_overflow=False), dtype=np.int16)

            # 音量
            raw_norm = np.linalg.norm(data) / CHUNK
            volume_voice = raw_norm / 5.0

            # 基频 pitch
            fft_data = np.fft.rfft(data)
            freqs = np.fft.rfftfreq(len(data), 1.0 / RATE)
            pitch_hz = freqs[np.argmax(np.abs(fft_data))]

            is_speech = volume_voice > 30.0

            latest_voice = {
                "volume": float(volume_voice),
                "pitch": float(pitch_hz),
                "isSpeech": bool(is_speech),
                "timestamp": time.time()
            }

            print(f"[DEBUG] volume={volume_voice:.3f}, pitch={pitch_hz:.1f}Hz")

    finally:
        stream.stop_stream()
        stream.close()
        p.terminate()

# ----------- 姿势捕获主线程 -----------
def run_pose():
    mp_pose = mp.solutions.pose
    pose = mp_pose.Pose(
        static_image_mode=False,
        model_complexity=1,
        enable_segmentation=False,
        min_detection_confidence=0.5,
        min_tracking_confidence=0.5
    )
    LANDMARK_NAMES = {lm.value: lm.name for lm in mp_pose.PoseLandmark}

    cap = cv2.VideoCapture(args.camera)
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, args.resize)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, int(args.resize * 3 / 4))

    try:
        while True:
            ret, frame = cap.read()
            if not ret or frame is None:
                print("Frame capture failed, retrying...")
                time.sleep(0.05)
                continue

            # ---- 发送视频 ----
            try:
                small_frame = cv2.resize(frame, (320, 240))   
                _, jpg = cv2.imencode('.jpg', small_frame, [cv2.IMWRITE_JPEG_QUALITY, 50])
                sock_video.sendto(jpg.tobytes(), video_addr)
            except Exception as e:
                print("Video send failed:", e)

            h, w = frame.shape[:2]
            image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

            try:
                results = pose.process(image_rgb)
            except Exception as e:
                print("Pose processing error:", e)
                continue

            # ---- 打包发送姿势数据 ----
            packet = {
                "timestamp": time.time(),
                "landmarks": None,
                "width": w,
                "height": h,
                "voice": latest_voice,
            }

            if results.pose_landmarks:
                lm = results.pose_landmarks.landmark
                packet["landmarks"] = {
                    LANDMARK_NAMES[i]: [
                        float(lm[i].x),
                        float(lm[i].y),
                        float(lm[i].z),
                        float(lm[i].visibility)
                    ]
                    for i in range(len(lm))
                }

            sock.sendto(json.dumps(packet).encode("utf-8"), addr)

            # 显示摄像头
            cv2.imshow("Pose + Voice", frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

    finally:
        cap.release()
        cv2.destroyAllWindows()
        pose.close()
        sock.close()
        sock_video.close()

# ----------- 启动线程 -----------
t_voice = threading.Thread(target=run_voice, daemon=True)
t_voice.start()
run_pose()
