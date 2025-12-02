namespace ZConnect.Models.Events
{
    public class UdpStatusChangedEventArgs : EventArgs, ICommunicationStatus
    {
        public UdpStatusEnum StatusType { get; set; }
        public byte[]? Data { get; set; }
        public UdpConnectionModel? Connection { get; set; }
        Enum ICommunicationStatus.StatusType => StatusType;
    }
}
