namespace ClientBlockChain.Entities.Abstract;

public abstract class HardwareInfoBase
{
    public string Name { get; protected set; } = "Unknown";
    public string Status { get; protected set; } = "Unknown";
    public double UsagePercentage { get; protected set; } = 0;
    public double TotalCapacity { get; protected set; } = 0;
}
