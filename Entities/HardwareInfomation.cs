using ClientBlockChain.Entities.HadwareInfo;

namespace ClientBlockChain.Entities;

public class HardwareInfomation
{
    public double Temperature { get; set; } 
    public int FanSpeed { get; set; }
    public CpuInfo CpuInfo { get; } = new CpuInfo();
    public GpuInfo GpuInfo { get; } = new GpuInfo();
    public MemoryInfo MemoryInfo { get; } = new MemoryInfo(); 
    public string TotalDiskSpace { get; } = DiskInfo.GetTotalDiskSpace();

}