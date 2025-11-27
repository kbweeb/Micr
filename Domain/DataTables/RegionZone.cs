namespace Domain.DataTables;

public class RegionZone
{
    public long RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? CreatedByUserId { get; set; }
    public DateTime? CreatedDate { get; set; }

    public ICollection<Bank> Banks { get; set; } = new List<Bank>();
}
