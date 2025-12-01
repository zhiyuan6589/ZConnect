using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ZConnect.Models;
using ZConnect.Models.Events;

namespace ZConnect.Services
{
    public class UdpClientService
    {
        public UdpConnectionModel Connection { get; } = new();

        public event EventHandler<UdpStatusChangedEventArgs>? StatusChanged;

        private UdpClient? _client;

        private CancellationTokenSource? _cts;

        public void Connect(string localIp, int localPort, string remoteIp, int remotePort)
        {
            try
            {
                _client = new UdpClient(new IPEndPoint(IPAddress.Parse(localIp), localPort));   // Bind local IP and Port by IPEndPoint.
                _client.Connect(remoteIp, remotePort);

                Connection.LocalIp = localIp;
                Connection.LocalPort = localPort;
                Connection.RemoteIp = remoteIp;
                Connection.RemotePort = remotePort;

                NotifyStatus(UdpStatusEnum.Started);

                _cts = new CancellationTokenSource();

                _ = ReceiveLoop(_cts.Token);
            }
            catch
            {
                _cts?.Cancel();
                _client?.Dispose();
                NotifyStatus(UdpStatusEnum.Stopped);
            }
        }

        public async Task ReceiveLoop(CancellationToken token)
        {
            if (_client == null) return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult received = await _client.ReceiveAsync();

                    Connection.RemoteIpEndPoint = received.RemoteEndPoint;  // received including the mew RemoteEndPoint.
                    Connection.LastReceived = received.Buffer;
                    Connection.LastActiveTime = DateTime.Now;

                    NotifyStatus(UdpStatusEnum.DataReceived, received.Buffer);
                }
                catch (Exception ex)
                {
                    NotifyStatus(UdpStatusEnum.Error);
                    Console.WriteLine(ex);
                    break;
                }
            }
        }

        public async Task SendAsync(byte[] data)
        {
            if (_client != null)
            {
                try
                {
                    await _client.SendAsync(data, data.Length);
                    NotifyStatus(UdpStatusEnum.DataSent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    NotifyStatus(UdpStatusEnum.Error);
                }
            }
        }

        public void Close()
        {
            _cts?.Cancel();
            _cts = null;

            _client?.Dispose();
            _client = null;
            NotifyStatus(UdpStatusEnum.Stopped);
        }

        public void NotifyStatus(UdpStatusEnum statusType, byte[]? data = null)
        {
            StatusChanged?.Invoke(this, new UdpStatusChangedEventArgs
            {
                StatusType = statusType,
                Data = data,
                Connection = Connection
            });
        }
    }
}
