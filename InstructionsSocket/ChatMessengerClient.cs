using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using ClientBlockchain.Entities;
using ClientBlockchain.SystemOperation;
using ClientBlockChain.Entities;

namespace ClientBlockChain.InstructionsSocket
{
    public class ChatMessengerClient
    {
        private readonly Socket _workSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public event Action<Socket>? StatusClientConnected;
        public event Action<ClientMine>? OnClientInfoReceived;
        public event Action<string>? OnMessageReceived;
        private const int CheckInterval = 20000;

        public ChatMessengerClient(Socket socket)
        {
            _workSocket = socket;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartChatAsync()
        {
            try
            {
                _ = MonitorConnectionAsync(_cancellationTokenSource.Token);

                await SendClientInfoAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no chat: {ex.Message}");
            }
        }

        private async Task MonitorConnectionAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _workSocket.Connected)
            {
                try
                {
                    await SendObjectAsync(new object
                    {
                    });
                    await ReceiveMessageAsync();
                }
                catch (SocketException)
                {
                    Console.WriteLine("Conexão perdida. Tentando reconectar...");
                    StatusClientConnected?.Invoke(_workSocket);
                    break;
                }
            }
        }

        private async Task ReceiveMessageAsync()
        {

            byte[] lengthBuffer = new byte[sizeof(int)];
            var receiveTask = _workSocket.ReceiveAsync(lengthBuffer, SocketFlags.None);

            var timeoutTask = Task.Delay(CheckInterval, _cancellationTokenSource.Token);

            var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            if (completedTask == timeoutTask) throw new SocketException();

            int bytesRead = await receiveTask;

            if (bytesRead == 0) throw new SocketException();

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] messageBuffer = new byte[messageLength];

            // Recebe o objeto completo
            receiveTask = _workSocket.ReceiveAsync(messageBuffer, SocketFlags.None);

            completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            if (completedTask == timeoutTask) throw new SocketException();

            bytesRead = await receiveTask;

            if (bytesRead > 0)
            {
                try
                {
                    var clientInfo = JsonSerializer.Deserialize<ClientMine>(messageBuffer);
                    if (clientInfo != null)
                    {
                        OnClientInfoReceived?.Invoke(clientInfo);
                        return;
                    }
                }
                catch
                {
                    string textMessage = Encoding.UTF8.GetString(messageBuffer, 0, bytesRead);
                    OnMessageReceived?.Invoke(textMessage);
                }
            }
        }

        private async Task SendClientInfoAsync()
        {
            var clientInfo = new ClientMine()
            {
                Status = true,
                Name = Environment.MachineName,
                So = IdentifierSystemOperation.GetOS().ToString(),
            };

            await SendObjectAsync(clientInfo);
        }

        public async Task SendObjectAsync<T>(T obj)
        {
            if (!_workSocket.Connected) return;

            var objectData = JsonSerializer.SerializeToUtf8Bytes(obj);
            var lengthData = BitConverter.GetBytes(objectData.Length);

            await _workSocket.SendAsync(lengthData, SocketFlags.None);
            await _workSocket.SendAsync(objectData, SocketFlags.None);

            // await SendDataWithTimeoutAsync(lengthData);
            // await SendDataWithTimeoutAsync(objectData);
        }

        private async Task SendDataWithTimeoutAsync(byte[] data)
        {
            var timeoutTask = Task.Delay(CheckInterval, _cancellationTokenSource.Token);

            var sendTask = _workSocket.SendAsync(data, SocketFlags.None);

            var completedTask = await Task.WhenAny(sendTask, timeoutTask);

            if (completedTask == timeoutTask) throw new SocketException();

            if (sendTask.IsFaulted) throw new SocketException();

        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
