namespace Domain.ViewModels.BookTypes;

public class BookTypeIndexViewModel
{
    public List<BookTypeListItemViewModel> Items { get; set; } = new();
}

public class BookTypeListItemViewModel
{
    public long BookTypeId { get; set; }
    public string BookTypeCode { get; set; } = string.Empty;
    public string BookTypeName { get; set; } = string.Empty;
    public long AccountTypeId { get; set; }
    public string? AccountTypeName { get; set; }
    public long NumberOfLeafletId { get; set; }
    public string? NumberOfLeaflet { get; set; }
    public long TransactionCodeId { get; set; }
    public string? TransactionCode { get; set; }
    public string Created { get; set; } = string.Empty;
}

public class BookTypeFormViewModel
{
    public long? BookTypeId { get; set; }
    public string BookTypeCode { get; set; } = string.Empty;
    public string BookTypeName { get; set; } = string.Empty;
    public long AccountTypeId { get; set; }
    public long NumberOfLeafletId { get; set; }
    public long TransactionCodeId { get; set; }
    public long? CreatedByUserId { get; set; }
}

public class BookTypeDto
{
    public long BookTypeId { get; set; }
    public string BookTypeCode { get; set; } = string.Empty;
    public string BookTypeName { get; set; } = string.Empty;
    public long AccountTypeId { get; set; }
    public string? AccountTypeName { get; set; }
    public long NumberOfLeafletId { get; set; }
    public string? NumberOfLeaflet { get; set; }
    public long TransactionCodeId { get; set; }
    public string? TransactionCode { get; set; }
    public string Created { get; set; } = string.Empty;
}
