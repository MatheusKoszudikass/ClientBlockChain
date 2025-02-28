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
    public class ClientMineService(
        IDataMonitorService<ClientMine> dataMonitorService) : IClientMineService
    {
        private readonly IDataMonitorService<ClientMine> _dataMonitorService = dataMonitorService;

        public async Task ClientMineInfoAsync(Listener listener)
        {
            var clientMine = new ClientMine();

           await _dataMonitorService.StartDepencenciesAsync(listener);
           await _dataMonitorService.SendDataAsync(clientMine);
        //    _= _dataMonitorService.ReceiveDataAsync();
        }
        
    }
}