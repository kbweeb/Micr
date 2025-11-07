namespace Domain.DataTables;

public class BankBranch
{
    public long BankBranchId { get; set; }
    public string BankBranchName { get; set; } = string.Empty;
    public long BankId { get; set; }

    public Bank Bank { get; set; } = null!;
}
