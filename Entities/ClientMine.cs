
using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using ClientBlockchain.Connection;
using ClientBlockchain.Entities;

namespace ClientBlockChain.Entities;
public class ClientMine()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Socket? Socket { get; set; }
    public SslStream? SslStream { get; set; }
    public string IpPublic { get; set; } = ConnectionHost.GetPublicIPAdress().Result;
    public string IpLocal { get; set; } = ConnectionHost.GetLocalIpAddress();
    public string Name { get; set; } = Environment.MachineName;
    public bool Status { get; set; }
    public string So { get; set; } = Environment.OSVersion.ToString();
    public int HoursRunning { get; set; }
    public HardwareInfomation HardwareInfo { get; set; } = new HardwareInfomation();
    public MiningStats Mining { get; set; } = new MiningStats();
}

