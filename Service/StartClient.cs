using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientBlockChain.Entity;
using ClientBlockChain.InstructionsSocket;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service
{
    public class StartClient(
        IDataConfirmationService dataConfirmationService, IClientMineService clientMineService) : IStartClient
    {
        private readonly IDataConfirmationService _dataConfirmationService = dataConfirmationService;
        private readonly IClientMineService _clientMineService = clientMineService;
        public async Task Connect()
        {
            var workSocket = new Listener(5000);

            try
            {
                await workSocket.Start();

                var chat = new ChatMessengerClient(workSocket.GetSocket());

                async Task Reconnect()
                {
                    workSocket.Stop();
                    workSocket = new Listener(5000);
                    await workSocket.Start();
                    chat = new ChatMessengerClient(workSocket.GetSocket());
                    RegisterReconnectEvent();
                    await chat.StartChatAsync();
                }

                void RegisterReconnectEvent()
                {
                    chat.StatusClientConnected -= async (socket) => await Reconnect();
                    chat.StatusClientConnected += async (socket) => await Reconnect();
                }

                RegisterReconnectEvent();

                // await _dataConfirmationService.StartMonitoringAsync(workSocket.GetSocket());

                await _clientMineService.ClientMineInfoAsync(workSocket);

                await _dataConfirmationService.Monitoring(workSocket);

                // await chat.StartChatAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar: {ex.Message}");
            }
        }

        private static void RegisterReconnectEvent()
        {
            chat.StatusClientConnected -= async (socket) => await Reconnect();
            chat.StatusClientConnected += async (socket) => await Reconnect();
        }
    }
}