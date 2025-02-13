using System.Net;
using System.Net.Sockets;

namespace ClientBlockChain.Entity
{
    public class Listener(uint port)
    {
        readonly Socket WorkSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static readonly string Ip = "192.168.0.2";
        public bool Listening { get; private set; }
        public uint Port { get; private set; } = port;

        public async Task Start()
        {
            if (this.Listening) return;

            var ip = new IPEndPoint(IPAddress.Any, checked((int)Port));

            await ConnectWithRetry();
            this.Listening = true;
        }


        private async Task ConnectWithRetry()
        {
            while (!WorkSocket.Connected)
            {
                try
                {
                    await this.WorkSocket.ConnectAsync(Ip, checked((int)Port));
                }
                catch (SocketException)
                {
                    Console.WriteLine("Erro ao conectar ao servidor.");
                    await Task.Delay(5000);
                }
            }
        }

        public Socket GetSocket()
        {
            return this.WorkSocket;
        }

        public void Stop()
        {
            if (!this.Listening) return;

            this.WorkSocket.Close();
            this.WorkSocket.DisconnectAsync(Listening);

            this.Listening = false;
        }
    }
}