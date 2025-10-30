using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.Dtos;

public class RegionUpsertRequest
{
    [Required]
    [MaxLength(100)]
    public string RegionName { get; set; } = string.Empty;

    public long? CreatedByUserId { get; set; }
}
