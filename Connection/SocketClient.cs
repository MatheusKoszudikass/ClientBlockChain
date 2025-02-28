using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using ClientBlockChain.Entity;
using ClientBlockChain.InstructionsSocket;

namespace ClientBlockchain.Connection;
public class SocketClient
{
    public static async Task<Socket> Connect()
    {
        var workSocket = new Listener(5000);

        try
        {
            var chat = new ChatMessengerClient(workSocket.GetSocket());

              async Task Reconnect()
            {
                workSocket.Stop();
                workSocket = new Listener(5000); // Cria um novo Listener
                await workSocket.Start();
                chat = new ChatMessengerClient(workSocket.GetSocket()); // Cria um novo ChatMessengerClient
                RegisterReconnectEvent(); // Re-registrar o evento após a reconexão
                await chat.StartChatAsync();
            }

            void RegisterReconnectEvent()
            {
                chat.StatusClientConnected -= async (socket) => await Reconnect(); // Remove o evento anterior
                chat.StatusClientConnected += async (socket) => await Reconnect(); // Adiciona o novo evento
            }

            RegisterReconnectEvent();

            await workSocket.Start();

            await chat.StartChatAsync();

            return workSocket.GetSocket();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao conectar: {ex.Message}");
            return workSocket.GetSocket();
        }
    }
}
