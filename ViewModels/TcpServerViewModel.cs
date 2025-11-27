using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    /// <summary>
    /// Tcp service ViewModel.
    /// Used to bind logic to UI.
    /// </summary>
    public class TcpServerViewModel : BaseTcpViewMode
    {
        public TcpServerViewModel(TcpServerService service) : base(service)
        {
            service.StatusChanged += OnStatusChanged;
        }

        public async Task StartAsync()
        {
            await ((TcpServerService)_service).StartAsync(Connection.LocalIp!, Connection.LocalPort);
        }

        public void Stop()
        {
            ((TcpServerService)_service).Stop();
        }
    }
}
