namespace Domain.ViewModels.BankBranches;

public class BankBranchIndexViewModel
{
    public List<BankBranchListItemViewModel> Items { get; set; } = new();
}

public class BankBranchListItemViewModel
{
    public long BankBranchId { get; set; }
    public string BankBranchName { get; set; } = string.Empty;
    public long BankId { get; set; }
    public string? BankName { get; set; }
    public bool IsEnabled { get; set; }
    public string Created { get; set; } = string.Empty;
}

public class BankBranchFormViewModel
{
    public long? BankBranchId { get; set; }
    public string BankBranchName { get; set; } = string.Empty;
    public long BankId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public long? CreatedByUserId { get; set; }
}

public class BankBranchDto
{
    public long BankBranchId { get; set; }
    public string BankBranchName { get; set; } = string.Empty;
    public long BankId { get; set; }
    public string? BankName { get; set; }
    public bool IsEnabled { get; set; }
    public string Created { get; set; } = string.Empty;
}
