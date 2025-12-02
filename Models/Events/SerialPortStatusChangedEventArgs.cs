namespace ZConnect.Models
{
    public class SerialPortStatusChangedEventArgs : EventArgs, ICommunicationStatus
    {
        public SerialPortStatusEnum StatusType { get; set; }
        public byte[]? Data { get; set; }
        public SerialPortConnectionModel? Connection { get; set; }

        // InterfaceName.MemberName => expression;
        Enum ICommunicationStatus.StatusType => StatusType;     // Here is the implementation of the interface, allowing the base class to use Enum to handle different StatusTypes
    }
}
