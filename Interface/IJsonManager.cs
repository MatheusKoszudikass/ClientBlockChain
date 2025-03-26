namespace ClientBlockChain.Interface;
public interface IJsonManager<T> 
{
    string Serialize(T objs);
    byte[] SerializeByte(T data);
    T Desserialize(string json);
    T Desserialize(byte[] data);
}