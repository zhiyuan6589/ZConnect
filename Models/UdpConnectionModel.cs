using System.Net;

namespace ZConnect.Models
{
    /// <summary>
    /// UDP connection data model.
    /// Store local/remote ip and port, and send/receive data.
    /// </summary>
    public class UdpConnectionModel
    {
        // Local Info
        public string LocalIp { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; } = 5000;

        // Remote Info
        public string RemoteIp { get; set; } = "127.0.0.1";
        public int RemotePort { get; set; } = 5000;

        public IPEndPoint? RemoteIpEndPoint;

        // Send/Receive Info
        public byte[]? LastSent { get; set; }
        public byte[]? LastReceived { get; set; }
        public DateTime LastActiveTime { get; set; }
    }
}
