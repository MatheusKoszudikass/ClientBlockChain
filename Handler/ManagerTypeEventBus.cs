using ServerBlockChain.Service;
using ServerBlockChain.Entities;

namespace ServerBlockChain.Handler;

public class ManagerTypeEventBus ()
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    
    public void PublishEventType(object data)
    {
        switch (data)
        {
            case ClientMine clientMine:
                _globalEventBus.Publish(clientMine);
                break;
            case ServerListener listener:
                _globalEventBus.Publish(listener);
                break;
            case LogEntry logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case SendMessageDefault sendMessageDefault:
                _globalEventBus.Publish(sendMessageDefault);
                break;
            default:
                throw new ArgumentException("Unsupported data type", nameof(data));
        }
    }
}