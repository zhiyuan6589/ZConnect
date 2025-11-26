using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Models;

public enum TcpStatusEnum
{
    Listening,
    NotListening,
    Connected,
    Disconnected,
    DataReceived,
    DataSent,
    Error,
}