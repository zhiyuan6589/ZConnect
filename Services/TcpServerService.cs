using System.Net;
using System.Net.Sockets;
using ZConnect.Models;
using ZConnect.ViewModels;

namespace ZConnect.Services
{
    /// <summary>
    /// Tcp server service.
    /// Handle tcp server connection/disconnection, send/receive data operations.
    /// </summary>
    public class TcpServerService : ITcpServiec
    {
        public TcpConnectionModel Connection { get; } = new();

        public event EventHandler<TcpStatusChangedEventArgs>? StatusChanged;    // An event that can be subscribed to.

        private TcpListener? _listener;

        private TcpClient? _client;

        public async Task StartAsync(string ip, int port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse(ip);
                _listener = new TcpListener(localAddr, port);
                _listener.Start();

                Connection.IsListening = true;
                Connection.IsConnected = false;
                Connection.LastActiveTime = DateTime.Now;

                NotifyStatus(TcpStatusEnum.Listening);

                _ = ListenerAsync();
            }
            catch
            {
                _listener?.Stop();
                Connection.IsListening = false;
                Connection.IsConnected = false;
                NotifyStatus(TcpStatusEnum.NotListening);
            }
        }

        private async Task ListenerAsync()
        {
            while (true)
            {
                _client = await _listener!.AcceptTcpClientAsync();
                Connection.IsConnected = true;
                NotifyStatus(TcpStatusEnum.Connected);

                var buffer = new byte[1024];
                var stream = _client.GetStream();

                while (Connection.IsConnected)
                {
                    try
                    {
                        int dataLength = await stream.ReadAsync(buffer);

                        if (dataLength == 0)
                        {
                            Connection.IsConnected = false;
                            NotifyStatus(TcpStatusEnum.Disconnected);
                            break;
                        }

                        byte[] received = new byte[dataLength];
                        Array.Copy(buffer, received, dataLength);

                        Connection.LastReceived = received;
                        Connection.LastActiveTime = DateTime.Now;
                        NotifyStatus(TcpStatusEnum.DataReceived, received);
                    }
                    catch
                    {
                        Connection.IsConnected = false;
                        break;
                    }
                }
            }
        }

        public async Task SendAsync(byte[] data)
        {
            if (_client?.Connected != true) return;
            try
            {
                await _client.GetStream().WriteAsync(data);

                Connection.LastSent = data;
                Connection.LastActiveTime = DateTime.Now;
                NotifyStatus(TcpStatusEnum.DataSent);
            }
            catch
            {
                NotifyStatus(TcpStatusEnum.Error);
            }
        }

        public void Stop()
        {
            _listener?.Stop();
            Connection.IsConnected = false;
            Connection.IsListening = false;
            NotifyStatus(TcpStatusEnum.NotListening);
        }

        public void NotifyStatus(TcpStatusEnum statusType, byte[]? data = null)
        {
            StatusChanged?.Invoke(this, new TcpStatusChangedEventArgs
            {
                StatusType = statusType,
                Data = data,
                Connection = Connection
            });
        }
    }
}
