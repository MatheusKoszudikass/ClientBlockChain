using ClientBlockChain.Entities;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockchain.Entities;

namespace ClientBlockchain.Service;

public class LoggerSendService : ILoggerSend
{
    private readonly List<LogEntry> _logEntries = [];
    private readonly ISend<LogEntry> _sendList;
    private readonly GlobalEventBus _globalEventBus;
    private readonly ManagerTypeEventBus _managerTypeEventBus = new ();
    private CancellationToken _cts = CancellationToken.None;

    public LoggerSendService(ISend<LogEntry> sendList,
     GlobalEventBus globalEventBus)
    {
        _sendList = sendList;
        _globalEventBus = globalEventBus;
        _globalEventBus.SubscribeList<LogEntry>(
            async logEntries => await OnLogEntryReceived(logEntries));
    }

    private async Task OnLogEntryReceived(List<LogEntry> logEntries)
    {
        try
        {
            _logEntries.AddRange(logEntries);
            Console.WriteLine($"Received LoggerSendService {_logEntries.Count} log entries.");
            await SendLogEntriesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing received log entry: {ex.Message}");
            _globalEventBus.Publish(Listener.Instance!);
            throw;
        }
    }

    public async Task SendLogEntriesAsync(CancellationToken cts = default)
    {
        try
        {
            if (_logEntries.Count >= 5)
            {
                var sslStream = await AuthenticateServer.AuthenticateAsClient(Listener.Instance.GetSocket(), cts);
                await _sendList.SendListAsync(
                    _logEntries, sslStream, _cts);

                _logEntries.Clear();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log entries: {ex.Message}");
            _managerTypeEventBus.PublishEventType(Listener.Instance!);
            throw;
        }
    }
}