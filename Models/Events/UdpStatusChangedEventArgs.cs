namespace ZConnect.Models.Events
{
    public class UdpStatusChangedEventArgs
    {
        public UdpStatusEnum StatusType { get; set; }
        public byte[]? Data { get; set; }
        public UdpConnectionModel? Connection { get; set; }
    }
}
