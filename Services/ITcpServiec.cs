using ZConnect.Models;

namespace ZConnect.ViewModels
{
    public interface ITcpServiec
    {
        TcpConnectionModel Connection { get; }
        event EventHandler<TcpStatusChangedEventArgs>? StatusChanged;
        Task SendAsync(byte[] data);
        void NotifyStatus(TcpStatusEnum statusType, byte[]? data = null);
    }
}
