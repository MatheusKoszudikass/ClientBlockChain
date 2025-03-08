using ClientBlockchain.Entities;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Handler;

public class ManagerTypeEventBus()
{
    private GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;

    public void PublishEventType(object data)
    {
        GlobalEventBusNewInstance();

        switch (data)
        {
            case ClientMine clientMine:
                _globalEventBus.Publish(clientMine);
                break;
            case Listener listener:
                _globalEventBus.Publish(listener);
                break;
            case LogEntry logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case SendMessageDefault sendMessageDefault:
                _globalEventBus.Publish(sendMessageDefault);
                break;
            case string message:
                _globalEventBus.Publish(message);
                break;
            default:
                throw new ArgumentException("Unsupported data type", nameof(data));
        }
    }

    public void PublishListEventType<T>(List<T> listData)
    {
        GlobalEventBusNewInstance();
        if (listData.Count == 0)
            throw new ArgumentException("List is empty", nameof(listData));


        switch (listData)
        {
            case List<ClientMine> clientMines:
                _globalEventBus.PublishList(clientMines);
                break;
            case List<Listener> Listeners:
                _globalEventBus.PublishList(Listeners);
                break;
            case List<LogEntry> logEntries:
                _globalEventBus.PublishList(logEntries);
                break;
            case List<SendMessageDefault> SendMessageDefaults:
                _globalEventBus.PublishList(SendMessageDefaults);
                break;
            case List<string> messages:
                _globalEventBus.PublishList(messages);
                break;
            default:
                throw new ArgumentException("Unsupported data type", nameof(listData));
        }
    }

    private void GlobalEventBusNewInstance() => _globalEventBus = GlobalEventBus.InstanceValue;
}