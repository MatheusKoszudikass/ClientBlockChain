namespace ClientBlockChain.Entities;

public class MiningStats
{
    public string HashMining { get; set; } = string.Empty;
    public double HashRate { get; set; }
    public int AcceptedShares { get; set; }
    public int RejectedShares { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CoinType { get; set; } = string.Empty;
    public double PowerConsumption { get; set; }

}
