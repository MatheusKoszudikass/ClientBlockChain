using ClientBlockChain.Entities.Abstract;
using Hardware.Info;

namespace ClientBlockChain.Entities.HadwareInfo;

public class MemoryInfo : HardwareInfoBase
{

    public MemoryInfo()
    {
        Status = "Active";
        GetMemoryInfo();
    }

    public void GetMemoryInfo()
    {
        GetRamUsage();
        GetRamTotal();
    }

    public void GetRamUsage()
    {
        var hardwareInfo = new HardwareInfo();
        hardwareInfo.RefreshMemoryStatus();
        var memoryStatus = hardwareInfo.MemoryStatus;
        UsagePercentage = (double)(memoryStatus.TotalPhysical - memoryStatus.AvailablePhysical)
        / memoryStatus.TotalPhysical * 100;
    }

    public void GetRamTotal()
    {
        var hardwareInfo = new HardwareInfo();
        hardwareInfo.RefreshMemoryStatus();
        var memoryStatus = hardwareInfo.MemoryStatus;
        TotalCapacity = (double)memoryStatus.TotalPhysical / (1024 * 1024);
    }
}
