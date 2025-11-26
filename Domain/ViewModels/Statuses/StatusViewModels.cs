namespace Domain.ViewModels.Statuses;

public class StatusIndexViewModel
{
    public List<StatusListItemViewModel> Items { get; set; } = new();
}

public class StatusListItemViewModel
{
    public long StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}

public class StatusFormViewModel
{
    public string StatusName { get; set; } = string.Empty;
    public long? CreatedByUserId { get; set; }
}

public class StatusDto
{
    public long StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}
