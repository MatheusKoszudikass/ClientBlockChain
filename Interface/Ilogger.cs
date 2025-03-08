using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;

namespace ClientBlockchain.Interface;
public interface IIlogger<T>
{
    Task Log(T data, Exception exception, string message, LogLevel level);
    Task Log(T data, string message, LogLevel level);
    Task Log(object data, Exception exception, string message, LogLevel level);
    Task Log(object data, string message, LogLevel level);
}
