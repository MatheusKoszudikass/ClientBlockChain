using ClientBlockChain.Entities;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockchain.Entities;
using ClientBlockChain.Entities.Enum;

namespace ClientBlockchain.Service;

public class LoggerSendService : ILoggerSend
{
    private readonly List<LogEntry> _logEntries = [];
    private readonly ISend<LogEntry> _sendList;
    private readonly GlobalEventBus _globalEventBus;

    public LoggerSendService(ISend<LogEntry> sendList,
     GlobalEventBus globalEventBus)
    {
        _sendList = sendList;
        _globalEventBus = globalEventBus;

        Task.Run(() =>
        {
            _globalEventBus.SubscribeList<LogEntry>(
                async logEntries => await OnLogEntryReceived(logEntries));
        });
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
            if (_logEntries.Count >= 4)
            {
                if (!Listener.Instance.Listening || !AuthenticateServer.SslStream!.IsAuthenticated) return;

                await _sendList.SendListAsync(
                    _logEntries, AuthenticateServer.SslStream, cts);

                Console.WriteLine($"Sent {_logEntries.Count} log entries.");

                _logEntries.Clear();
            }
        }
        catch (IOException)
        {
            Console.WriteLine("Server disconnected");
            _globalEventBus.Publish(Listener.Instance!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log entries: {ex.Message}");
            _globalEventBus.Publish(Listener.Instance!);
            throw;
        }
    }
}