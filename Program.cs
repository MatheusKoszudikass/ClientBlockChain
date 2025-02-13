
using ClientBlockchain.Connection;
using ClientBlockChain.InstructionsSocket;
using System.Net.Sockets;

class Program
{
    static async Task Main(string[] args)
    {
        var connection = new ConnectionHost();
        await connection.InitializeConnection();
        var socketServer = await SocketClient.Connect();
        Console.WriteLine("chegou");
        await ChatMessengerClient.StartChatAsync(socketServer);

        Console.ReadLine();
    }
}