using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Handler;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;

public class LoggerSendService : ILoggerSend
{
    private readonly List<LogEntry> _logEntries = [];
    private readonly ISend<LogEntry> _sendListLog;
    private readonly ISend<ClientCommandLog> _sendCommandList;
    private readonly GlobalEventBus _globalEventBus;
    private bool _primarySend;

    public LoggerSendService(ISend<LogEntry> sendList,
                             ISend<ClientCommandLog> sendCommandList,
                             GlobalEventBus globalEventBus)
    {
        _sendListLog = sendList;
        _sendCommandList = sendCommandList;
        _globalEventBus = globalEventBus;

        _globalEventBus.SubscribeList<LogEntry>(
            async logEntries => await OnLogEntryReceived(logEntries));

        _globalEventBus.Subscribe<ClientCommandLog>(OnLogCommandServer);
    }

    private async Task SendLogEntriesAutomaticallyAsync(CancellationToken cts = default)
    {
        if (_logEntries.Count >= 4 && !_primarySend)
        {
            await SendLogs(cts);
            _primarySend = true;
        }
    }

    private async Task SendLogEntriesByCommandAsync(CancellationToken cts = default)
    {
        if (_logEntries.Count == 0)
        {
            await _sendCommandList.SendAsync(ClientCommandLog.Empty, cts);
            return;
        }
        await SendLogs(cts);
    }

    private async Task SendLogs(CancellationToken cts)
    {
        if (!Listener.Instance.Listening || !AuthenticateServer.SslStream!.IsAuthenticated) return;

        await _sendListLog.SendListAsync(_logEntries, cts);
        _logEntries.Clear();
    }

    private async Task OnLogEntryReceived(List<LogEntry> logEntries)
    {
        try
        {
            _logEntries.AddRange(logEntries);
            await SendLogEntriesAutomaticallyAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing received log entry: {ex.Message}");
            _globalEventBus.Publish(Listener.Instance!);
            throw;
        }
    }

    private void OnLogCommandServer(ClientCommandLog clientCommandLog)
    {
        if (clientCommandLog == ClientCommandLog.Send)
        {
            _ = SendLogEntriesByCommandAsync();
        }
    }
}
