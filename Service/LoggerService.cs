using System.Net;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Handler;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;

public class LoggerService<T> : IIlogger<T>
{
    private static List<LogEntry> _logEntries = [];

    private readonly GlobalEventBus _globalEventBus;

    public LoggerService(GlobalEventBus globalEventBus)
    {
        _globalEventBus = globalEventBus;
        _globalEventBus.Subscribe<HttpStatusCode>(OnReceivedHttpStatusCode);
    }

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
            Console.WriteLine($"Log entry: {logEntry.Exception}");
            _globalEventBus.Publish(Listener.Instance);
            return;
        }
        _globalEventBus.PublishList(_logEntries!);
        _logEntries.Clear();
        await Task.FromResult(true);
    }

    private void OnReceivedHttpStatusCode(HttpStatusCode httpStatusCode)
    {
        switch (httpStatusCode)
        {
            case HttpStatusCode.OK:
                _= Log(HttpStatusCode.OK, "Request successful", LogLevel.Information);
                break;
            default:
                _globalEventBus.Publish(Listener.Instance);
                break;
        }
        if (httpStatusCode == HttpStatusCode.Unauthorized)
        {
            _globalEventBus.Publish(Listener.Instance);
        }
        if (httpStatusCode == HttpStatusCode.Forbidden)
        {
            _globalEventBus.Publish(Listener.Instance);
        }
    }
}

