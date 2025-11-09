using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PoseReceiver : MonoBehaviour
{
    public int port = 5005;  // Python UDP 端口
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
        public Dictionary<string, float[]> landmarks; // key = "LEFT_WRIST", value = [x,y,z,visibility]
        public VoiceData voice;                      // 新增：语音数据
        public float timestamp;
        public int width;
        public int height;
    }

    public PoseData latestPose;  // 存储最近一次完整数据

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
                byte[] data = udp.Receive(ref anyIP);
                string json = Encoding.UTF8.GetString(data);
                latestPose = JsonConvert.DeserializeObject<PoseData>(json);

                // 输出调试
                int count = latestPose?.landmarks?.Count ?? 0;
                float volume = latestPose?.voice?.volume ?? 0f;
                float pitch = latestPose?.voice?.pitch ?? 0f;
                bool isSpeech = latestPose?.voice?.isSpeech ?? false;
                // Debug.Log($"Received landmarks={count}, volume={volume:F2},pitch={pitch:F2}, isSpeech={isSpeech}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("UDP receive error: " + e);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();
        udp.Close();
    }
}
