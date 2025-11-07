namespace Domain.DataTables;

public class Bank
{
    public long BankId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string SortCode { get; set; } = string.Empty;
    public long RegionId { get; set; }

    public ICollection<BankBranch> BankBranches { get; set; } = new List<BankBranch>();
    public RegionZone Region { get; set; } = null!;
}
