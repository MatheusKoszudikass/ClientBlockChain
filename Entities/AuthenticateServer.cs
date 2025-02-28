using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Entities;

public class AuthenticateServer()
{
    private static SslStream? _sslStream;

    public static async Task<SslStream> AuthenticateAsClient(Socket socket, int timeoutMilliseconds = 10000)
    {
        if (_sslStream != null && _sslStream.IsAuthenticated) return _sslStream;

        var networkStream = new NetworkStream(socket);

        // Resolve o domínio para o endereço IP
        var domain = await Dns.GetHostEntryAsync("monerokoszudikas.duckdns.org");
        var ip = domain.AddressList[0].ToString();

        // Cria o SslStream
        _sslStream = new SslStream(networkStream, false, ValidateServerCertificate!, null);

        // Autentica o servidor com timeout
        var authenticateTask = _sslStream.AuthenticateAsClientAsync(ip, null, SslProtocols.Tls12, false);
        if (await Task.WhenAny(authenticateTask, Task.Delay(timeoutMilliseconds)) == authenticateTask)
        {
            // A tarefa de autenticação foi concluída dentro do tempo limite
            await authenticateTask; // Propaga exceções, se houver
        }
        else
        {
            // O tempo limite foi atingido antes da autenticação ser concluida
            Console.WriteLine("Timeout during authentication");
        }

        return _sslStream;
    }

    private static bool ValidateServerCertificate(
            object? sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}