namespace ZConnect.Models
{
    public class SerialPortStatusChangedEventArgs
    {
        public SerialPortStatusEnum StatusType { get; set; }
        public byte[]? Data { get; set; }
        public SerialPortConnectionModel? Connection { get; set; }
    }
}
