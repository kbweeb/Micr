using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class NumberOfLeafletCreateRequest
{
    [Required]
    [MaxLength(100)]
    public string Leaflet { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }
}
