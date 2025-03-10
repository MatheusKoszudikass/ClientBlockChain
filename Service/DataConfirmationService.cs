using ClientBlockchain.Entities;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;
public class DataConfirmationService : IDataConfirmationService
{
    private readonly IDataMonitorService<string> _dataMonitorService;
    private readonly IEventApp<SendMessageDefault> _eventApp;
    private readonly IIlogger<DataConfirmationService> _logger;
    private GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private static Listener? _listener;

    public DataConfirmationService(
        IDataMonitorService<string> dataMonitorService,
        IIlogger<DataConfirmationService> logger, IEventApp<SendMessageDefault> events)
    {
        _dataMonitorService = dataMonitorService;
        _logger = logger;
        _eventApp = events;
        _globalEventBus.Subscribe<string>(OnReceived);
    }

    public async Task StartMonitoringAsync(Listener listener,
    CancellationToken cts)
    {
        try
        {
            GlobalEventBusNewInstance();
            _eventApp.Sent += (data) =>
            {
                _logger.Log(listener, "StartMonitoringAsync event", LogLevel.Event);
                Console.WriteLine("StartMonitoringAsync event");

            };

            _eventApp.Received += (data) =>
            {
                _logger.Log(listener, "StartMonitoringAsync event", LogLevel.Event);
            };

            await _dataMonitorService.ReceiveDataAsync(cts);
            _listener = listener;

        }
        catch (Exception)
        {
            await _logger.Log(new SendMessageDefault(), "StartMonitoringAsync", LogLevel.Information);
            _eventApp.OnServerDisconnected(_listener!);
        }
    }

    public async Task Monitoring(Listener listener, CancellationToken cts)
    {
        try
        {
            GlobalEventBusNewInstance();

            while (listener.Listening)
            {
                await _dataMonitorService.ReceiveDataAsync(cts);
                await Task.Delay(5000);
            }
        }
        catch (Exception)
        {
            await _logger.Log(new SendMessageDefault(), "StartMonitoringAsync", LogLevel.Information);
            _eventApp.OnServerDisconnected(_listener!);
        }
    }

    private void OnReceived(string message)
    {
        Console.WriteLine($"Received message: {message}");
    }

    private void GlobalEventBusNewInstance()
    {
        _globalEventBus.Subscribe<string>(OnReceived);
    }
}
