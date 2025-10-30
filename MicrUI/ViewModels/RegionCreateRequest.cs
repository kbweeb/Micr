using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class RegionCreateRequest
{
    [Required]
    [MaxLength(150)]
    public string RegionName { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }
}
