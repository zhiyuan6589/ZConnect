using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows.Documents;
using ZConnect.Models;
using ZConnect.ViewModels;

namespace ZConnect.Services
{
    /// <summary>
    /// Tcp client service.
    /// Handle tcp client connection/disconnection, send/receive data operations.
    /// </summary>
    public class TcpClientService : ITcpServiec
    {
        public TcpConnectionModel Connection { get; } = new();

        public event EventHandler<TcpStatusChangedEventArgs>? StatusChanged;    // An event that can be subscribed to.

        private TcpClient? _client;

        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            try
            {
                await _client.ConnectAsync(ip, port);

                Connection.RemoteIp = ip;
                Connection.RemotePort = port;
                Connection.IsConnected = true;
                Connection.LastActiveTime = DateTime.Now;

                NotifyStatus(TcpStatusEnum.Connected);

                _ = ReceiveAsync(); // Start an asynchronous task. The return value is ignored, and the asynchornous method does not need to wait for completion.
            } 
            catch
            {
                _client?.Close();
                Connection.IsConnected = false;
                NotifyStatus(TcpStatusEnum.Disconnected);
            }
        }

        private async Task ReceiveAsync()
        {
            // The fixed-size buffer for receiving data is set to 1024 bytes (1k) here, you can also use other size.
            var buffer = new byte[1024];

            // ! is null-forgiving operator.
            var stream = _client!.GetStream();

            while (Connection.IsConnected)
            {
                try
                {
                    // Executing stream.ReadAsync(buffer) will write the read data into the buffer and return the length of the data. Here, the buffer is passed into the ReadAsync function as a container of data.
                    int dataLength = await stream.ReadAsync(buffer); // The length of bytes read from the stream this time, length depends on sender, the maximum is 1024.
                    if (dataLength > 0)
                    {
                        byte[] received = new byte[dataLength];
                        Array.Copy(buffer, received, dataLength);

                        Connection.LastReceived = received;
                        Connection.LastActiveTime = DateTime.Now;
                        NotifyStatus(TcpStatusEnum.DataReceived, received);
                    }
                    else
                    {
                        Connection.IsConnected = false;
                        NotifyStatus(TcpStatusEnum.Disconnected);
                        break;
                    }
                }
                catch
                {
                    Connection.IsConnected = false;
                    NotifyStatus(TcpStatusEnum.Disconnected);
                    break;
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

        public void Disconnect()
        {
            _client?.Close();
            Connection.IsConnected = false;
            NotifyStatus(TcpStatusEnum.Disconnected);
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
