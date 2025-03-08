namespace ClientBlockchain.Interface;

public interface ILoggerSend
{
    Task SendLogEntriesAsync(CancellationToken cts = default);
}