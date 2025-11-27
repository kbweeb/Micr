namespace Domain.ViewModels.Regions;

public class RegionIndexViewModel
{
    public List<RegionListItemViewModel> Items { get; set; } = new();
}

public class RegionListItemViewModel
{
    public long RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
    public int Banks { get; set; }
    public int Branches { get; set; }
}

public class RegionFormViewModel
{
    public long? RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? CreatedByUserId { get; set; }
}

public class RegionDto
{
    public long RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
    public int Banks { get; set; }
    public int Branches { get; set; }
}
