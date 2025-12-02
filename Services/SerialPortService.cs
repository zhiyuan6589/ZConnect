using System.IO.Ports;
using ZConnect.Models;

namespace ZConnect.Services
{
    public class SerialPortService
    {
        public SerialPortConnectionModel Connection { get; } = new SerialPortConnectionModel();

        public event EventHandler<SerialPortStatusChangedEventArgs>? StatusChanged;

        private SerialPort? _serialPort;

        private CancellationTokenSource? _cts;

        public void Open(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshark)
        {
            try
            {
                _serialPort = new SerialPort();
                _serialPort.PortName = portName;
                _serialPort.BaudRate = baudRate;
                _serialPort.Parity = parity;
                _serialPort.DataBits = dataBits;
                _serialPort.StopBits = stopBits;
                _serialPort.Handshake = handshark;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;
                _serialPort.Open();

                NotifyStatus(SerialPortStatusEnum.Opened);

                _cts = new CancellationTokenSource();

                _ = ReadAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _cts?.Cancel();
                _serialPort?.Dispose();
                Console.WriteLine(ex);
                NotifyStatus(SerialPortStatusEnum.Error);
            }
        }

        private async Task ReadAsync(CancellationToken token)
        {
            if (_serialPort == null) return;

            while (!token.IsCancellationRequested && _serialPort.IsOpen)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int count = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (count > 0)
                    {
                        byte[] received = [.. buffer.Take(count)];
                        Connection.LastReceived = received;
                        Connection.LastActiveTime = DateTime.Now;
                        NotifyStatus(SerialPortStatusEnum.DataReceived, received);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    NotifyStatus(SerialPortStatusEnum.Error);
                    break;
                }
            }
        }

        public async Task WriteAsync(byte[] data)
        {
            if (_serialPort != null)
            {
                try
                {
                    await _serialPort.BaseStream.WriteAsync(data);
                    NotifyStatus(SerialPortStatusEnum.DataSent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    NotifyStatus(SerialPortStatusEnum.Error);
                }
            }
        }

        public void Close()
        {
            _cts?.Cancel();
            _serialPort?.Dispose();
            _cts = null;
            _serialPort = null;
            NotifyStatus(SerialPortStatusEnum.Closed);
        }

        public static List<string> GetAvailablePorts()
        {
            return [.. SerialPort.GetPortNames().ToList()];
        }

        public void NotifyStatus(SerialPortStatusEnum statusType, byte[]? data = null)
        {
            StatusChanged?.Invoke(this, new SerialPortStatusChangedEventArgs
            {
                StatusType = statusType,
                Data = data,
                Connection = Connection
            });
        }
    }
}
