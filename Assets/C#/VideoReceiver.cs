using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class VideoReceiver : MonoBehaviour
{
    public RawImage rawImage;
    public int port = 5007;   // 必须和 Python 的 video_port 相同

    private UdpClient udp;
    private Texture2D tex;

    void Start()
    {
        udp = new UdpClient(port);
        tex = new Texture2D(2, 2);  // 小尺寸，后面会自动扩展
    }

    void Update()
    {
        if (udp.Available > 0)
        {
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
            byte[] data = udp.Receive(ref anyIP);

            // JPG → Texture2D
            tex.LoadImage(data);

            // 显示在 RawImage 上
            rawImage.texture = tex;
        }
    }

    void OnApplicationQuit()
    {
        udp.Close();
    }
}
