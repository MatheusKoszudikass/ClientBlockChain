using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientBlockChain.Entities;
using ClientBlockChain.Entity;

namespace ClientBlockChain.Interface
{
    public interface IDataConfirmationService
    {
        public event Action<SendMessageDefault>? DataAct;
        Task StartMonitoringAsync(Listener listener); 

        Task Monitoring(Listener listener);
    }
}