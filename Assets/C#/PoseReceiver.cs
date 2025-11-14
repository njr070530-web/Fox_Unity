using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PoseReceiver : MonoBehaviour
{
    public int port = 5005;
    private UdpClient udp;
    private Thread receiveThread;

    [System.Serializable]
    public class VoiceData
    {
        public bool isSpeech;
        public float timestamp;
        public float volume;
        public float pitch;
    }

    [System.Serializable]
    public class PoseData
    {
        public Dictionary<string, float[]> landmarks;
        public VoiceData voice;
        public float timestamp;
        public int width;
        public int height;
    }

    public PoseData latestPose;

    void Start()
    {
        udp = new UdpClient(port);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);

        while (true)
        {
            try
            {
                if (udp == null)  // ⭐ 防止已经被 OnDestroy 关闭时继续 Receive
                    return;

                byte[] data = udp.Receive(ref anyIP);
                if (data == null || data.Length == 0)
                    continue;

                string json = Encoding.UTF8.GetString(data);
                if (string.IsNullOrEmpty(json))
                    continue;

                // ⭐ 解析 JSON（可能为 null）
                PoseData pose = JsonConvert.DeserializeObject<PoseData>(json);
                if (pose == null)
                {
                    Debug.LogWarning("⚠ Received invalid JSON (pose == null)");
                    continue;
                }

                // ⭐ landmarks 可能为 null，确保不报错
                if (pose.landmarks == null)
                {
                    pose.landmarks = new Dictionary<string, float[]>();
                }

                latestPose = pose;

                // (可选) 调试输出安全访问
                // int count = pose.landmarks.Count;
                // float volume = pose.voice?.volume ?? 0f;
            }
            catch (SocketException)
            {
                // ⭐ udp.Close() 后会进这里，不再报错
                Debug.Log("UDP socket closed, thread ending.");
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("UDP receive error (handled): " + e.Message);
                Thread.Sleep(5); // ⭐ 防止刷屏
            }
        }
    }

    void OnApplicationQuit()
    {
        if (udp != null)
        {
            udp.Close();
            udp = null;
        }
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();
    }

    void OnDestroy()
    {
        if (udp != null)
        {
            udp.Close();
            udp = null;
        }
    }
}
