using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Models.Events
{
    public class TcpStatusChangedEventArgs : EventArgs
    {
        public TcpStatusType StatusType { get; set; }
        public string? Message { get; set; }
        public byte[]? Data { get; set; }
        public TcpConnectionModel? Connection { get; set; }

    }
}
