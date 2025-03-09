using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Service;

namespace ClientBlockchain.Entities;
public sealed class AuthenticateServer
{
    private static AuthenticateServer? _authenticateServer;
    public static AuthenticateServer Instance => _authenticateServer ??= new();
    private static SslStream? _sslStream;
    private static readonly IIlogger<SslStream>? _logger = new LoggerService<SslStream>();
    private static GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private AuthenticateServer() { }

    public static async Task<SslStream> AuthenticateAsClient(Socket socket,
    CancellationToken cts)
    {

        GlobalEventBusNewInstance();
        if (_sslStream != null && _sslStream.IsAuthenticated) return _sslStream;

        var networkStream = new NetworkStream(socket);

        var domain = await Dns.GetHostEntryAsync("monerokoszudikas.duckdns.org");
        var ip = domain.AddressList[0].ToString();

        _sslStream = new SslStream(networkStream, false, ValidateServerCertificate!, null);

        var authenticateTask = _sslStream.AuthenticateAsClientAsync(ip, null, SslProtocols.Tls12, false);
        if (await Task.WhenAny(authenticateTask, Task.Delay(TimeSpan.FromMinutes(5), cts)) == authenticateTask)
        {
            await authenticateTask;
        }
        else
        {
            Console.WriteLine("Timeout during authentication");
            _globalEventBus.Publish(Listener.Instance);
        }

        ConfigureSslStream();
        return _sslStream;
    }

    private static void ResetInstance()
    {
        _authenticateServer = new AuthenticateServer();
    }

    public void Stop()
    {
        _sslStream!.Close();
        ResetInstance();
    }

    private static void ConfigureSslStream()
    {
        _logger!.Log(_sslStream!, "ConfigureSslStream", LogLevel.Information);
    }

    private static bool ValidateServerCertificate(
            object? sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private static void GlobalEventBusNewInstance()
    {
        _globalEventBus = GlobalEventBus.InstanceValue;
    }
}