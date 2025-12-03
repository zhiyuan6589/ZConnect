using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZConnect.Models;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    public partial class BaseCommunicationViewModel : ObservableObject    // Interface that notifies the UI that "a certain property has changed."
    {
        [ObservableProperty]
        private string _status = "None";

        [ObservableProperty]
        private string _receivedText = "";  // Private property, access and update through the public property ReceivedText.

        [ObservableProperty]
        private string _sendText = "";

        [ObservableProperty]
        private bool _autoSend = false;

        [ObservableProperty]
        private int _intervalMs = 1000;

        [ObservableProperty]
        private DataFormatEnum _sendFormat = DataFormatEnum.String;

        [ObservableProperty]
        private DataFormatEnum _receiveFormat;

        protected Func<byte[], Task>? SendAction;   // C# 内置的委托类型，Func<byte[], Task> 表示接收 byte[] 类型的参数， 返回 Task 方法，简言之是一个可以异步调用的函数

        protected bool _isAutoSending = false;

        [RelayCommand]
        private void ClearText()
        {
            ReceivedText = "";
        }

        [RelayCommand]
        private async Task Send()
        {
            if (_isAutoSending || string.IsNullOrEmpty(SendText)) return;

            byte[] data = FormatConverter.ConvertSend(SendText, SendFormat);

            _isAutoSending = AutoSend;
            do
            {
                if (SendAction != null) await SendAction(data);

                ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {FormatConverter.ConvertReceived(data, SendFormat)}\n";    // The recived/send text record.
                if (!AutoSend) break;
                await Task.Delay(IntervalMs).ConfigureAwait(false);
            } while (AutoSend);
            _isAutoSending = false;
        }

        protected void OnStatusChanged(object? sender, ICommunicationStatus args)
        {
            if (args.Data != null)
            {
                string text = FormatConverter.ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            Status = args.StatusType.ToString();
        }
    }
}
