using System.IO.Ports;

namespace ZConnect.Models
{
    public class SerialPortConnectionModel
    {
        public string? PortName { get; set; }
        public int BaudRate { get; set; } = 115200;
        public Parity Parity { get; set; } = Parity.None;    // 校验方式，枚举类型
        public int DataBits { get; set; } = 8;  // 数据位的数量（5/6/7/8，通常为8）
        public StopBits StopBits { get; set; } = StopBits.One;   // 停止位，枚举类型
        public Handshake Handshake { get; set; } = Handshake.None; // 流控制，枚举类型

        public byte[]? LastSent { get; set; }
        public byte[]? LastReceived { get; set; }
        public DateTime LastActiveTime { get; set; }
    }
}
