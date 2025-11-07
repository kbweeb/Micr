namespace Domain.DTOs;

public class RegionIndexDto
{
    public long RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
    public int Banks { get; set; }
    public int Branches { get; set; }
}
