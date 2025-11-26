using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Models;

public class TcpStatusChangedEventArgs : EventArgs
{
    public TcpStatusEnum StatusType { get; set; }
    public byte[]? Data { get; set; }
    public TcpConnectionModel? Connection { get; set; }

}
