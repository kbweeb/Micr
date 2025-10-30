namespace MicrDbChequeProcessingSystem.Dtos;

public record AccountTypeDto(
    long Id,
    string Name,
    string Code,
    bool IsActive,
    DateTime CreatedDate
);
