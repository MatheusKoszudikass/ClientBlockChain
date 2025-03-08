using ClientBlockChain.Entities;

namespace ClientBlockchain.Interface;

public interface ILoggerSend
{
    Task SendLogEntriesAsync(List<LogEntry> logEntries);
}