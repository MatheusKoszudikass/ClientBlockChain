using System.Net;
using System.Net.Sockets;

namespace ClientBlockchain.Entities;

public sealed class Listener
{
    private static Listener? _listenerInstance;
    public static Listener Instance => _listenerInstance ??= new();
    private Socket _workSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static readonly IPHostEntry Domain = Dns.GetHostEntryAsync("monerokoszudikas.duckdns.org").Result;
    private static readonly IPAddress Ip = Domain.AddressList[0];
    public bool Listening { get; set; }
    private uint Port { get; set; } = 5000;

    private Listener() { }

    public async Task Start()
    {
        if (this.Listening) return;

        this.Listening = true;

        await ConnectWithRetry();
    }

    private async Task ConnectWithRetry()
    {
        while (true)
        {
            try
            {
                if (!this._workSocket.Connected)
                {
                    Console.WriteLine($"Tentando conectar ao servidor... {Ip}");
                    await this._workSocket.ConnectAsync(Ip, checked((int)Port));
                    Console.WriteLine("Conectado ao servidor!");
                    break;
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Error de conexão. Tentando novamente em 5 segundos...");
                await Task.Delay(5000);
            }
        }
    }

    public async Task Reconnect()
    {
        this.Listening = false;
        this._workSocket.Close();
        this._workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await ConnectWithRetry();
    }

    private static void ResetInstance()
    {
        _listenerInstance = new Listener();
    }


    public Socket GetSocket()
    {
        return this._workSocket;
    }

    public void Stop()
    {
        if (!this.Listening) return;
        this._workSocket.Close();
        this.Listening = false;
        ResetInstance();
    }

    private void OnStatusClientConnected(Socket e) => StatusClientConnected?.Invoke(this, e);

    public event EventHandler<Socket>? StatusClientConnected;
}