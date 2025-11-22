using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Models
{
    public enum TcpStatusType
    {
        Connected,
        Disconnected,
        DataReceived,
        DataSent,
        Error
    }
}
