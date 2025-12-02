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