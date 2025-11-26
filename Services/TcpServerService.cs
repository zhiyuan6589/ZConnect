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
            IPAddress localAddr = IPAddress.Parse(ip);
            _server = new TcpListener(localAddr, port);
            try
            {
                _server.Start();

                Connection.LocalIp = ip;
                Connection.LocalPort = port;
                Connection.IsListening = true;
                Connection.IsConnected = false;
                Connection.LastActiveTime = DateTime.Now;

                NotifyStatus(TcpStatusEnum.Listening, "Listening!");

                _ = ListenerAsync();
            }
            catch
            {
                Connection.IsListening = false;
                Connection.IsConnected = false;
                NotifyStatus(TcpStatusEnum.NotListening, "Not listening!");
            }
        }

        private async Task ListenerAsync()
        {
            while (true)
            {
                _client = await _server!.AcceptTcpClientAsync();
                Connection.IsConnected = true;
                NotifyStatus(TcpStatusEnum.Connected, "Connected!");

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
                            NotifyStatus(TcpStatusEnum.Disconnected, "Disconnected!");
                            break;
                        }

                        byte[] received = new byte[dataLength];
                        Array.Copy(buffer, received, dataLength);

                        Connection.LastReceived = received;
                        Connection.LastActiveTime = DateTime.Now;

                        NotifyStatus(TcpStatusEnum.DataReceived, "Received data!", received);
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
                NotifyStatus(TcpStatusEnum.DataSent, "Sent data!");
            }
            catch
            {
                NotifyStatus(TcpStatusEnum.Error, "Sent data failed!");
            }
        }

        public void Stop()
        {
            _server?.Stop();
            Connection.IsConnected = false;
            Connection.IsListening = false;
            NotifyStatus(TcpStatusEnum.NotListening, "NotListening!");
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
