using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using ClientBlockchain.Connection;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Ultil;

namespace ClientBlockChain.Entities;
public class LogEntry
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Ip { get; set; } = ConnectionHost.GetPublicIPAdress().Result;
    public string Level {get; set; } = string.Empty;
    public string Message {get; set; } = "Client";
    public string ApplicationName { get; set;} = string.Empty;
    public string MachineName { get; } = Environment.MachineName;
    public string ProcessId { get; } = Environment.ProcessId.ToString();
    public string Architecture { get; } = RuntimeInformation.OSArchitecture.ToString();
    public string Version { get; } = Environment.Version.ToString();
    public string UserName { get; } = Environment.UserName;
    public string UserDomain { get; } = Environment.UserDomainName;
    public string ThreadId { get; } = Environment.CurrentManagedThreadId.ToString();
    public string? Exception { get; set; }
    
    [JsonConverter(typeof(ObjectToStringConverter))]
    public object? Source {get; set;}
    public string? StackTrace { get; set; }
    [JsonIgnore] public IntPtr Handler { get; set; }
}