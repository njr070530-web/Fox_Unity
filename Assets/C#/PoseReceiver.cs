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

    private static PoseReceiver instance;   // ⭐ 保证全局唯一
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

    // -------------------------------------------------
    //  Awake：实现单例 + 跨场景保留
    // -------------------------------------------------
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);   // ⭐ 如果已有一个，则销毁重复的
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -------------------------------------------------
    //  Start：初始化 UDP
    // -------------------------------------------------
    void Start()
    {
        udp = new UdpClient(port);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // -------------------------------------------------
    //  后台线程读取
    // -------------------------------------------------
    void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);

        while (true)
        {
            try
            {
                if (udp == null) return;

                byte[] data = udp.Receive(ref anyIP);
                if (data == null || data.Length == 0) continue;

                string json = Encoding.UTF8.GetString(data);
                if (string.IsNullOrEmpty(json)) continue;

                PoseData pose = JsonConvert.DeserializeObject<PoseData>(json);
                if (pose == null)
                {
                    Debug.LogWarning("⚠ invalid JSON (pose == null)");
                    continue;
                }

                if (pose.landmarks == null)
                    pose.landmarks = new Dictionary<string, float[]>();

                latestPose = pose;
            }
            catch (SocketException)
            {
                Debug.Log("UDP socket closed, thread ending.");
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("UDP error (handled): " + e.Message);
                Thread.Sleep(5);
            }
        }
    }

    // -------------------------------------------------
    //  生命周期清理
    // -------------------------------------------------
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void OnDestroy()
    {
        CloseSocket();
    }

    // -------------------------------------------------
    //  统一关闭逻辑
    // -------------------------------------------------
    private void CloseSocket()
    {
        if (udp != null)
        {
            udp.Close();
            udp = null;
        }

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
    }
}
