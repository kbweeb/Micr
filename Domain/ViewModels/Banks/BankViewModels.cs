namespace Domain.ViewModels.Banks;

public class BankIndexViewModel
{
    public List<BankListItemViewModel> Items { get; set; } = new();
}

public class BankListItemViewModel
{
    public long BankId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string SortCode { get; set; } = string.Empty;
    public long RegionId { get; set; }
    public string? RegionName { get; set; }
    public bool IsEnabled { get; set; }
    public string Created { get; set; } = string.Empty;
}

public class BankFormViewModel
{
    public string BankName { get; set; } = string.Empty;
    public string SortCode { get; set; } = string.Empty;
    public long RegionId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public long? CreatedByUserId { get; set; }
}

public class BankDto
{
    public long BankId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string SortCode { get; set; } = string.Empty;
    public long RegionId { get; set; }
    public string? RegionName { get; set; }
    public bool IsEnabled { get; set; }
    public string Created { get; set; } = string.Empty;
}
