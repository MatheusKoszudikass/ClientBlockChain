using System.Text;
using System.Text.Json;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;

namespace ClientBlockchain.Service;

public class JsonManager<T> : IJsonManager<T> where T : class
{
    private readonly ManagerTypeEventBus _managerTypeEventBus = new ManagerTypeEventBus();
    
    public string Serialize(T obj)
    {
        var result = JsonSerializer.Serialize(obj);
        _managerTypeEventBus.PublishEventType(obj);
        return result;
    }

    public byte[] SerializeByte(T obj)
    {
        var result = JsonSerializer.SerializeToUtf8Bytes(obj);
        return result;
    }
    
    public T Desserialize(string json)
    {
        var result = JsonSerializer.Deserialize<T>(json) ?? null!;
        _managerTypeEventBus.PublishEventType(result);
        return result;
    }

    public T Desserialize(byte[] data)
    {
        var obj = Encoding.UTF8.GetString(data);
        var result = JsonSerializer.Deserialize<T>(obj) ?? null!;
        _managerTypeEventBus.PublishEventType(result);
        return result;
    }
}