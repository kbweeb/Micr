namespace MicrDbChequeProcessingSystem.Dtos;

public record BankDto(
    long Id,
    string Name,
    string SortCode,
    long RegionId,
    string RegionName,
    bool IsEnabled,
    int BranchCount,
    DateTime? CreatedDate
);
