using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Text;
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

        public event Action<byte[]>? DataReceived;  // An event that can be subscribed to.

        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            Connection.RemoteIp = ip;
            Connection.RemotePort = port;
            Connection.IsConnected = true;
            Connection.LastActiveTime = DateTime.Now;

            _ = ReceiveAsync();
        }

        public async Task SendAsync(byte[] data)
        {
            if (_client?.Connected != true) return;
            await _client.GetStream().WriteAsync(data);

            Connection.LastSent = data;
            Connection.LastActiveTime = DateTime.Now;
        }

        private async Task ReceiveAsync()
        {
            // The fixed-size buffer for receiving data is set to 1024 bytes (1k) here, you can also use other size.
            var buffer = new byte[1024];    

            // ! is null-forgiving operator.
            var stream = _client!.GetStream();

            while (Connection.IsConnected)
            {
                // Executing stream.ReadAsync(buffer) will write the read data into the buffer and return the length of the data. Here, the buffer is passed into the ReadAsync function as a container of data.
                int dataLength = await stream.ReadAsync(buffer); // The length of bytes read from the stream this time, length depends on sender, the maximum is 1024.
                if (dataLength > 0)
                {
                    byte[] received = new byte[dataLength];
                    Array.Copy(buffer, received, dataLength);

                    Connection.LastRecived = received;
                    Connection.LastActiveTime = DateTime.Now;

                    DataReceived?.Invoke(received);
                }
            }
        }

        public void Disconnect()
        {
            _client?.Close();
            Connection.IsConnected = false;
        }
    }
}
