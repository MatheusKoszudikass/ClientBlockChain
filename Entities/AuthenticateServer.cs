using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Handler;
using ClientBlockChain.Interface;
using ClientBlockChain.Service;

namespace ClientBlockChain.Entities;
public class AuthenticateServer
{
    private static AuthenticateServer? _authenticateServer;
    public static AuthenticateServer Instance => _authenticateServer ??= new();
    public static SslStream? SslStream { get; private set; }
    private static readonly IIlogger<SslStream>? _logger =
     new LoggerService<SslStream>(GlobalEventBus.InstanceValue);

    private static GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    public AuthenticateServer() { }

    public static async Task AuthenticateAsClient(Socket socket,
    CancellationToken cts = default)
    {
        var networkStream = new NetworkStream(socket);

        var domain = await Dns.GetHostEntryAsync("monerokoszudikas.duckdns.org", cts);

        SslStream = new SslStream(networkStream, false, RemoteCertificateValidationCallback, null);

        var authenticateTask = SslStream.AuthenticateAsClientAsync(domain.HostName, null,
        SslProtocols.Tls12 | SslProtocols.Tls13, true);

        if (await Task.WhenAny(authenticateTask, Task.Delay(TimeSpan.FromMinutes(5), cts)) == authenticateTask)
        {
            await authenticateTask;
        }
        else
        {
            await _logger!.Log(SslStream!, "Timeout during authentication", LogLevel.Information);
            throw new AuthenticationException("Timeout during authentication");
        }

        await SendGuidTokenServer();
        ConfigureSslStream();
    }

    private static bool RemoteCertificateValidationCallback(
        object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            _logger!.Log(SslStream!, $"Certificate error: {sslPolicyErrors}", LogLevel.Error);
            _globalEventBus.Publish(Listener.Instance);
            return false;
        }
        return ValidateCertificate(chain!, certificate);
    }

    private static async Task SendGuidTokenServer()
    {
        var send = new SendService<GuidTokenAuth>();
        var toke = new GuidTokenAuth();
        await send.SendAsync(toke);
    }

    private static void OnReceivedHttpStatusCode(HttpStatusCode httpStatusCode)
    {
        if (httpStatusCode == HttpStatusCode.Unauthorized)
        {
            _globalEventBus.Publish(Listener.Instance);
        }
        if (httpStatusCode == HttpStatusCode.Forbidden)
        {
            _globalEventBus.Publish(Listener.Instance);
        }
    }

    private static bool ValidateCertificate(X509Chain chain, X509Certificate? certificate)
    {
        if (chain != null)
        {
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

            if (!chain.Build((X509Certificate2)certificate!))
            {
                foreach (var status in chain.ChainStatus)
                {
                    _logger!.Log(SslStream!, $"Certificate error: {status.StatusInformation}", LogLevel.Error);
                }
                _globalEventBus.Publish(Listener.Instance);

                return false;
            }
        }
        return true;
    }

    private static void ResetInstance()
    {
        _authenticateServer = new AuthenticateServer();
    }

    public void Stop()
    {
        SslStream!.Close();
        ResetInstance();
    }

    private static void ConfigureSslStream()
    {
        _logger!.Log(SslStream!, "ConfigureSslStream", LogLevel.Information);
    }
}