using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientBlockChain.InstructionsSocket
{
    public class ChatMessengerClient
    {
        public static async Task StartChatAsync(Socket clientSocket)
        {
            try
            {
                // Inicia a escuta contínua em uma thread separada
                Task receiveTask = Task.Run(() => ListenForMessagesAsync(clientSocket));

                while (clientSocket.Connected)
                {
                    await SendMessageAsync(clientSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no chat: {ex.Message}");
            }
            // finally
            // {
            //     clientSocket.Close();
            //     Console.WriteLine("Conexão encerrada.");
            // }
        }

        private static async Task ListenForMessagesAsync(Socket clientSocket)
        {
            try
            {
                while (clientSocket.Connected)
                {
                    byte[] buffer = new byte[1024];
                    Console.Write("> ");
                    int bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);

                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"[Servidor]: {message}");
                        continue;
                    }
                    break;
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Conexão com o servidor perdida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao receber mensagem: {ex.Message}");
            }
        }

        private static async Task SendMessageAsync(Socket clientSocket)
        {
            try
            {
   
                string? message = await Task.Run(() => Console.ReadLine());
                Console.Write("> ");
                if (!string.IsNullOrWhiteSpace(message))
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await clientSocket.SendAsync(messageBytes, SocketFlags.None);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Erro na conexão com o servidor.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
            }
        }
    }
}
