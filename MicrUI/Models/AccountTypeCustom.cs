using System;

namespace MicrDbChequeProcessingSystem.Models;

public class AccountTypeCustom
{
    public int Id { get; set; }

    public string AccountTypeName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
