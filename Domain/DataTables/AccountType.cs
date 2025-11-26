using System;

namespace Domain.DataTables;

public class AccountType
{
    public long AccountTypeId { get; set; }
    public string AccountTypeCode { get; set; } = null!;
    public string AccountTypeName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public long? CreatedByUserId { get; set; }
    public DateTime? CreatedDate { get; set; }
}
