using ClientBlockchain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockchain.Handler;
using ClientBlockChain.InstructionsSocket;
using ClientBlockchain.Interface;
using ClientBlockChain.Interface;
using ClientBlockchain.Service;

namespace ClientBlockChain.Service;

public class StartClient : IStartClient
{
    private readonly IClientMineService _clientMineService;
    private readonly IIlogger<Listener> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private bool _isREconnecting = false;
    private int _retryCount = 0;

    public StartClient(IClientMineService clientMineService,
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

            _ = _logger.Log(Listener.Instance, "Connected to server", LogLevel.Information);

            await _clientMineService.ClientMineInfoAsync(Listener.Instance);
        }
        catch (Exception ex)
        {
            _ = _logger.Log(Listener.Instance!, ex, ex.Message, LogLevel.Error);
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
            Console.WriteLine($"Retry count reconnecting: {_retryCount}");
            AuthenticateServer.Instance.Stop();
            Listener.Instance.Stop();
            _retryCount++;

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