namespace Domain.DTOs;

public class RegionCreateDto
{
    public string RegionName { get; set; } = null!;
    public string? Description { get; set; }
    public long? CreatedByUserId { get; set; }
}

public class RegionDto
{
    public long RegionId { get; set; }
    public string RegionName { get; set; } = null!;
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
}
