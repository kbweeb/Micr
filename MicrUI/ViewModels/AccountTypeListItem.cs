namespace MicrDbChequeProcessingSystem.ViewModels;

public class AccountTypeListItem
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Description { get; set; }

    public string Created { get; set; } = string.Empty;
}
