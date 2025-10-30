using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.Dtos;

public class AccountTypeUpsertRequest
{
    [Required]
    [MaxLength(100)]
    public string AccountTypeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string AccountTypeCode { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public long? CreatedByUserId { get; set; }
}
