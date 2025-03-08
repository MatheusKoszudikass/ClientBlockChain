namespace ClientBlockChain.Entities;
public static class StateObject
{
    public static int BufferSize = 5;
    public static int BufferReceiveSize;
    public static byte[] BufferInit { get; set; } = new byte[BufferSize];
    public static byte[] BufferReceive { get; set; } = new byte[BufferReceiveSize];
    public static bool IsList { get; set; }
    public static byte[] BufferSend { get; set; } = [];
}