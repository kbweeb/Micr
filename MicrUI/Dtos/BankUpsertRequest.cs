using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.Dtos;

public class BankUpsertRequest
{
    [Required]
    [MaxLength(100)]
    public string BankName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SortCode { get; set; } = string.Empty;

    [Required]
    public long RegionId { get; set; }

    public bool? IsEnabled { get; set; }

    public long? CreatedByUserId { get; set; }
}
