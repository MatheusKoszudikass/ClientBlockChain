using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientBlockChain.InstructionsSocket;

namespace ClientBlockChain.Entity
{
    public class Listener(uint port)
    {
        private Socket WorkSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly IPHostEntry Domain = Dns.GetHostEntryAsync("monerokoszudikas.duckdns.org").Result;
        public static readonly IPAddress Ip = Domain.AddressList[0];
        public bool Listening { get; private set; }
        public uint Port { get; private set; } = port;

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
                    if (!this.WorkSocket.Connected)
                    {
                        Console.WriteLine($"Tentando conectar ao servidor... {Ip}");
                        await this.WorkSocket.ConnectAsync(Ip, checked((int)Port));
                        Console.WriteLine("Conectado ao servidor!");
                        break;
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Erro de conexão. Tentando novamente em 5 segundos...");
                    await Task.Delay(5000);
                }
            }
        }

        public async Task Reconnect()
        {
            this.Listening = false;
            this.WorkSocket.Close();
            this.WorkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await ConnectWithRetry();
        }

        public Socket GetSocket()
        {
            return this.WorkSocket;
        }

        public void Stop()
        {
            if (!this.Listening) return;

            this.WorkSocket.Close();

            this.Listening = false;
        }

        protected virtual void OnStatusClientConnected(Socket e) => StatusClientConnected?.Invoke(this, e);

        public event EventHandler<Socket>? StatusClientConnected;
    }
}