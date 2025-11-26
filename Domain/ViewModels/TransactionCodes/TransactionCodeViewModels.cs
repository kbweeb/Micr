namespace Domain.ViewModels.TransactionCodes;

public class TransactionCodeIndexViewModel
{
    public List<TransactionCodeListItemViewModel> Items { get; set; } = new();
}

public class TransactionCodeListItemViewModel
{
    public long TransactionCodeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}

public class TransactionCodeFormViewModel
{
    public string Code { get; set; } = string.Empty;
    public long? CreatedByUserId { get; set; }
}

public class TransactionCodeDto
{
    public long TransactionCodeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}
