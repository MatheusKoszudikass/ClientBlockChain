using ClientBlockChain.Entities.Enum;

namespace ClientBlockChain.Interface;
public interface IIlogger<in T>
{
    Task Log(T data, Exception exception, string message, LogLevel level);
    Task Log(T data, string message, LogLevel level);
    Task Log(object data, Exception exception, string message, LogLevel level);
    Task Log(object data, string message, LogLevel level);
}
