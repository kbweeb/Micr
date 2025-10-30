namespace MicrDbChequeProcessingSystem.Dtos;

public record BankBranchDto(
    long Id,
    string Name,
    long BankId,
    string BankName,
    bool IsEnabled,
    DateTime? CreatedDate
);
