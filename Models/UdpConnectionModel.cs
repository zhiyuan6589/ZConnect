using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ZConnect.Models
{
    public class UdpConnectionModel
    {
        // Local Info
        public string LocalIp { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; } = 5000;

        // Remote Info
        public string RemoteIp { get; set; } = "127.0.0.1";
        public int RemotePort { get; set; } = 5000;

        public IPEndPoint? RemoteIpEndPoint;

        public byte[]? LastSent { get; set; }
        public byte[]? LastReceived { get; set; }
        public DateTime LastActiveTime { get; set; }
    }
}
