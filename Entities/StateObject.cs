namespace ClientBlockChain.Entities;
public class StateObject
{
    public int BufferSize { get; set; }
    public byte[] BufferInit { get; set; } = new byte[5];
    public byte[] BufferSend { get; set; } = [];
    public byte[] BufferReceive { get; set; } = [];
    public bool IsList { get; set; }
}

