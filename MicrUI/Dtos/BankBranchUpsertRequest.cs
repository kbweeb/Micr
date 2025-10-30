using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.Dtos;

public class BankBranchUpsertRequest
{
    [Required]
    [MaxLength(100)]
    public string BankBranchName { get; set; } = string.Empty;

    [Required]
    public long BankId { get; set; }

    public bool IsEnabled { get; set; } = true;

    public long? CreatedByUserId { get; set; }
}
