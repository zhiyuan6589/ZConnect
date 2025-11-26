using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ZConnect.Models;
using System.Net;

namespace ZConnect.Services
{
    public class TcpServerService
    {
        public TcpConnectionModel Connection { get; private set; } = new TcpConnectionModel();
        private TcpListener? _server;
        private TcpClient? _client;

        public event EventHandler<TcpStatusChangedEventArgs>? StatusChanged;

        public async Task StartAsync(string ip, int port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse(ip);
                _server = new TcpListener(localAddr, port);
                _server.Start();

                Connection.LocalIp = ip;
                Connection.LocalPort = port;
                Connection.IsListening = true;
                Connection.IsConnected = false;
                Connection.LastActiveTime = DateTime.Now;

                NotifyStatus(TcpStatusEnum.Listening);

                _ = ListenerAsync();
            }
            catch
            {
                _server?.Stop();
                Connection.IsListening = false;
                Connection.IsConnected = false;
                NotifyStatus(TcpStatusEnum.NotListening);
            }
        }

        private async Task ListenerAsync()
        {
            while (true)
            {
                _client = await _server!.AcceptTcpClientAsync();
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
            _server?.Stop();
            Connection.IsConnected = false;
            Connection.IsListening = false;
            NotifyStatus(TcpStatusEnum.NotListening);
        }

        private void NotifyStatus(TcpStatusEnum statusType, byte[]? data = null)
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
