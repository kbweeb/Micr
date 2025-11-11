using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class StatusCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string StatusName { get; set; } = string.Empty;

    // Description not present on Status; keep minimal like source model
}
