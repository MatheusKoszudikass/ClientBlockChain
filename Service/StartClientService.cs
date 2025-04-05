using ClientBlockChain.Handler;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Interface;
using System.Net.Sockets;
using System.Security.Authentication;

namespace ClientBlockChain.Service;

public class StartClientService : IStartClient
{
    private readonly IClientMine _clientMineService;
    private readonly IIlogger<Listener> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private bool _isREconnecting = false;

    public StartClientService(IClientMine clientMineService,
    IIlogger<Listener> logger)
    {
        _clientMineService = clientMineService;
        _logger = logger;
        _globalEventBus.Subscribe<Listener>(HandleDisconnection);
    }

    public async Task Connect()
    {
        try
        {
            await Listener.Instance.Start();

            await AuthenticateServer.AuthenticateAsClient(Listener.Instance.GetSocket());

            await _logger.Log(Listener.Instance, "Connected to server", LogLevel.Information);
            
            await _clientMineService.ClientMineInfoAsync();
        }
        catch (AuthenticationException ex)
        {
            await _logger.Log(Listener.Instance!, ex,
            $"Authentication server failed: {ex.Message}", LogLevel.Error);
            await Reconnect();
        }
        catch (InvalidOperationException ex)
        {
            await _logger.Log(Listener.Instance!, ex, 
            $"Trying to connect socket already connected "+
            $"or some operation was invalid: {ex.Message}", LogLevel.Error);
            await Reconnect();
        }
        catch (SocketException ex)
        {
            await _logger.Log(Listener.Instance!, ex, 
            $"Error trying to access the socket: {ex.Message}", LogLevel.Error);
            await Reconnect();
        }
        catch (IOException)
        {
            await _logger.Log(Listener.Instance!, "Server disconnected", LogLevel.Error);
            await Reconnect();
        }
        catch (Exception ex)
        {
            await _logger.Log(Listener.Instance!, ex, ex.Message, LogLevel.Error);
            await Reconnect();
        }
    }

    private async Task Reconnect()
    {
        if (_isREconnecting) return;

        _isREconnecting = true;
        await _semaphore.WaitAsync();

        try
        {
            AuthenticateServer.Instance.Stop();
            Listener.Instance.Stop();

            GlobalEventBus.ResetInstance();
            _globalEventBus = GlobalEventBus.InstanceValue;
            _globalEventBus.Subscribe<Listener>(HandleDisconnection);
            _isREconnecting = false;
            await Connect();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async void HandleDisconnection(Listener listener)
    {
        await Reconnect();
    }
}