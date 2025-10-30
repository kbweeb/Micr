using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class AccountTypeCreateRequest
{
    [Required]
    [MaxLength(150)]
    public string AccountTypeName { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }
}
