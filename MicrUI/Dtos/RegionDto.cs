namespace MicrDbChequeProcessingSystem.Dtos;

public record RegionDto(
    long Id,
    string Name,
    int BankCount,
    int BranchCount,
    DateTime? CreatedDate
);
