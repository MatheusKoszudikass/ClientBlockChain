
using ClientBlockChain.Connection;

namespace ClientBlockChain.Entities;
public class ClientMine()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientInfoId { get; set; } = GuidToken.GuidTokenGlobal;
    public string IpPublic { get; set; } = ConnectionHost.GetPublicIPAdress().Result.TrimEnd('\n', '\r');
    public string IpLocal { get; set; } = Listener.Instance.GetSocket().LocalEndPoint!.ToString()!.Split(':')[0];
    public string Name { get; set; } = Environment.MachineName;
    public bool IsStatus { get; set; } = true;
    public bool IsStatusMining { get; set; } = false;
    public string So { get; set; } = Environment.OSVersion.ToString();
    public int HoursRunning { get; set; }
    public HardwareInfomation? HardwareInfo { get; set; } 
    public MiningStats Mining { get; set; } = new MiningStats();
}

