namespace Domain.ViewModels.AccountTypes;

public class AccountTypeIndexViewModel
{
    public List<AccountTypeListItemViewModel> Items { get; set; } = new();
}

public class AccountTypeListItemViewModel
{
    public long AccountTypeId { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public string? AccountTypeCode { get; set; }
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
}

public class AccountTypeFormViewModel
{
    public long? AccountTypeId { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? CreatedByUserId { get; set; }
}

public class AccountTypeDto
{
    public long AccountTypeId { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public string? AccountTypeCode { get; set; }
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
}
