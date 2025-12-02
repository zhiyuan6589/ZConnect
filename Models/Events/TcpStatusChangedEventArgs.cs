namespace ZConnect.Models;

public class TcpStatusChangedEventArgs : EventArgs
{
    public TcpStatusEnum StatusType { get; set; }
    public byte[]? Data { get; set; }
    public TcpConnectionModel? Connection { get; set; }
}
