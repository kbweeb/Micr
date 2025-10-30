using System.ComponentModel.DataAnnotations;

namespace MicrDbChequeProcessingSystem.ViewModels;

public class ReportIssueViewModel
{
    [Required]
    [Display(Name = "Your name")]
    public string ReporterName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Work email")]
    public string ReporterEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Affected module")]
    public string Module { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, ErrorMessage = "Please keep descriptions within 2000 characters.")]
    [Display(Name = "Issue description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Attach reference ID (optional)")]
    public string? ReferenceId { get; set; }
}
