using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class TransactionCodeCreateRequest
{
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }
}
