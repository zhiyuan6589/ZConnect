using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Net.Sockets;
using System.Text;
using System.Windows.Documents;
using ZConnect.Models;

namespace ZConnect.Services
{
    /// <summary>
    /// Tcp client service.
    /// Handle tcp client connection/disconnection, send/receive data operations.
    /// </summary>
    public class TcpClientService
    {
        public TcpConnectionModel Connection { get; private set; } = new TcpConnectionModel();
        private TcpClient? _client;

        public event EventHandler<TcpStatusChangedEventArgs>? StatusChanged;    // An event that can be subscribed to.

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

                NotifyStatus(TcpStatusEnum.Connected, "Connection successful!");

                _ = ReceiveAsync(); // Start an asynchronous task. The return value is ignored, and the asynchornous method does not need to wait for completion.
            } 
            catch
            {
                Connection.IsConnected = false;
                NotifyStatus(TcpStatusEnum.Error, "Connection error!");
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
                NotifyStatus(TcpStatusEnum.DataSent, "Sent data!");
            }
            catch
            {
                NotifyStatus(TcpStatusEnum.Error, "Sent data failed!");
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

                        NotifyStatus(TcpStatusEnum.DataReceived, "Received data!", received);
                    }
                    else
                    {
                        Connection.IsConnected = false;
                        NotifyStatus(TcpStatusEnum.Disconnected, "Disconnected!");
                        break;
                    }
                }
                catch
                {
                    Connection.IsConnected = false;
                    NotifyStatus(TcpStatusEnum.Error, "Connection error!");
                    break;
                }
            }
        }

        public void Disconnect()
        {
            _client?.Close();
            Connection.IsConnected = false;
            NotifyStatus(TcpStatusEnum.Disconnected, "Disconnected!");
        }

        private void NotifyStatus(TcpStatusEnum statusType, string message, byte[]? data = null)
        {
            StatusChanged?.Invoke(this, new TcpStatusChangedEventArgs
            {
                StatusType = statusType,
                Message = message,
                Data = data,
                Connection = Connection
            });
        }
    }
}
