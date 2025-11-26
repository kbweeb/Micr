namespace Domain.ViewModels.NumberOfLeaflets;

public class NumberOfLeafletIndexViewModel
{
    public List<NumberOfLeafletListItemViewModel> Items { get; set; } = new();
}

public class NumberOfLeafletListItemViewModel
{
    public long NumberOfLeafletId { get; set; }
    public string NumberOfLeaflet { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}

public class NumberOfLeafletFormViewModel
{
    public string NumberOfLeaflet { get; set; } = string.Empty;
    public long? CreatedByUserId { get; set; }
}

public class NumberOfLeafletDto
{
    public long NumberOfLeafletId { get; set; }
    public string NumberOfLeaflet { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}
