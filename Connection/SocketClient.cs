using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using ClientBlockChain.Entity;

namespace ClientBlockchain.Connection;
public class SocketClient
{
    public static async Task<Socket> Connect()
    {
        // var socketClient = new Socket(AddressFamily.InterNetwork, 
        // SocketType.Stream, ProtocolType.Tcp);
         var workSocket = new Listener(5000);
        //  workSocket.SocketConnected += (sender, args) => 
        // {
        //     AddConnectedSocket(sender, args);
        // };

        try
        {

            await workSocket.Start();
            // var connectionHost = new ConnectionHost();

            // while(!socketClient.Connected)
            // {
            //    socketClient.Connect(ConnectionHost.GetPublicIPAdress().Result, 5000);
            //   Console.WriteLine("Conectado ao servidor.");
            // }

            return workSocket.GetSocket();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao conectar: {ex.Message}"); 
            return workSocket.GetSocket();
        }
    }

    public static void AddConnectedSocket(Object? obj, SocketConnectedEventHandler args)
    {
        Console.WriteLine($"Conexão feita com o IP: {args.SocketConnected.RemoteEndPoint}");
    }
}
