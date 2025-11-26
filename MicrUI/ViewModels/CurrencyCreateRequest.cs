using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class CurrencyCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string CurrencyName { get; set; } = string.Empty;

    [MaxLength(3)]
    public string? CurrencyCode { get; set; }

    [MaxLength(10)]
    public string? Symbol { get; set; }
}
