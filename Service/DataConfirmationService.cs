using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientBlockChain.Entities;
using ClientBlockChain.Entity;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service
{
    public class DataConfirmationService(
         IDataMonitorService<object> dataMonitorService) : IDataConfirmationService
    {
        private readonly IDataMonitorService<object> _dataMonitorService = dataMonitorService;
        public event Action<SendMessageDefault>? DataAct;
        private static Listener? _listener;

        public async Task StartMonitoringAsync(Listener listener)
        {
            await _dataMonitorService.StartDepencenciesAsync(listener);
            await _dataMonitorService.SendDataAsync(SendMessageDefault.MessageSuccess);
            await _dataMonitorService.ReceiveDataAsync();
            _listener = listener;
        }

        public async Task Monitoring(Listener listener)
        {
            while (listener.Listening)
            {
                await _dataMonitorService.StartDepencenciesAsync(listener);
                await _dataMonitorService.ReceiveDataAsync();
                await _dataMonitorService.SendDataAsync(SendMessageDefault.MessageSuccess);
                await Task.Delay(5000);
            }
        }
    }
}