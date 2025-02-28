using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientBlockChain.Entity;

namespace ClientBlockChain.Interface
{
    public interface IDataMonitorService<T>
    {
        Task StartDepencenciesAsync(Listener listener);
        Task ReceiveDataAsync();
        Task SendDataAsync(T data);
        void StopMonitoring();
    }
}