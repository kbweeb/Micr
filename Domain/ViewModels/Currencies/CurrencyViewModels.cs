namespace Domain.ViewModels.Currencies;

public class CurrencyIndexViewModel
{
    public List<CurrencyListItemViewModel> Items { get; set; } = new();
}

public class CurrencyListItemViewModel
{
    public long CurrencyId { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public string? CurrencyCode { get; set; }
    public string? Symbol { get; set; }
    public bool IsActive { get; set; }
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}

public class CurrencyFormViewModel
{
    public string CurrencyName { get; set; } = string.Empty;
    public string? CurrencyCode { get; set; }
    public string? Symbol { get; set; }
    public long? CreatedByUserId { get; set; }
}

public class CurrencyDto
{
    public long CurrencyId { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public string? CurrencyCode { get; set; }
    public string? Symbol { get; set; }
    public bool IsActive { get; set; }
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}
