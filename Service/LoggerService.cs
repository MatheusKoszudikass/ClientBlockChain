using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockchain.Entities;

namespace ClientBlockChain.Service;
public class LoggerService<T> : IIlogger<T>
{
    private static List<LogEntry> _logEntries = [];
    private readonly ManagerTypeEventBus _managerTypeEventBus = new();

    public async Task Log(T data, Exception exception, string message, LogLevel level)
    {
        var logEntry = new LogEntry
        {
            Level = level.ToString(),
            Message = message,
            Exception = exception.ToString(),
            Source = data,
            StackTrace = exception.StackTrace!
        };

        _logEntries.Add(logEntry);

        await SendLogEntries(logEntry);
    }

    public async Task Log(T data, string message, LogLevel level)
    {
        var logEntry = new LogEntry
        {
            Level = level.ToString(),
            Message = message,
            Exception = null,
            Source = data,
            StackTrace = null
        };

        _logEntries.Add(logEntry);

        await SendLogEntries(logEntry);
    }

    public async Task Log(object data, Exception exception, string message, LogLevel level)
    {
        var logEntry = new LogEntry
        {
            Level = level.ToString(),
            Message = message,
            Exception = exception.ToString(),
            Source = data,
            StackTrace = exception.StackTrace!
        };

        _logEntries.Add(logEntry);

        await SendLogEntries(logEntry);
    }

    public async Task Log(object data, string message, LogLevel level)
    {
        var logEntry = new LogEntry
        {
            Level = level.ToString(),
            Message = message,
            Exception = null,
            Source = data,
            StackTrace = null
        };

        _logEntries.Add(logEntry);

        await SendLogEntries(logEntry);
    }

    private async Task SendLogEntries(LogEntry logEntry)
    {
        if (logEntry.Exception != null)
        {
            _managerTypeEventBus.PublishEventType(Listener.Instance);
            return;
        }
        _managerTypeEventBus.PublishListEventType(_logEntries!);
        _logEntries.Clear();
        await Task.FromResult(true);
    }
}

