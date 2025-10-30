using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class Currency
{
    public long CurrencyId { get; set; }

    public string? CurrencyCode { get; set; }

    public string CurrencyName { get; set; } = null!;

    public string? Symbol { get; set; }

    public bool IsActive { get; set; }

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual UserProfile CreatedByUser { get; set; } = null!;
}
