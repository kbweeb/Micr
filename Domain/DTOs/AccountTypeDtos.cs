namespace Domain.DTOs;

public class AccountTypeCreateDto
{
    public string AccountTypeName { get; set; } = null!;
    public string? Description { get; set; }
    public long CreatedByUserId { get; set; }
}

public class AccountTypeDto
{
    public long AccountTypeId { get; set; }
    public string AccountTypeName { get; set; } = null!;
    public string AccountTypeCode { get; set; } = null!;
    public string? Description { get; set; }
    public string Created { get; set; } = string.Empty;
}
