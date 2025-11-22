using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Models
{
    /// <summary>
    /// TCP connection data model.
    /// Store local/remote IP addresses and ports, and last sent/received data.
    /// </summary>
    public class TcpConnectionModel
    {
        // Local Info
        public string? LocalIp { get; set; }
        public int LocalPort { get; set; }

        // Remote Info
        public string? RemoteIp { get; set; }
        public int RemotePort { get; set; }

        // Status
        public bool IsConnected { get; set; }

        // Sent/Recived Data
        public byte[]? LastSent { get; set; }
        public byte[]? LastReceived { get; set; }
        public DateTime LastActiveTime { get; set; }

    }
}
