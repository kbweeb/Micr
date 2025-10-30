using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class Bank
{
    public long BankId { get; set; }

    public string SortCode { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public long RegionId { get; set; }

    public bool? IsEnabled { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<BankBranch> BankBranches { get; set; } = new List<BankBranch>();

    public virtual UserProfile? CreatedByUser { get; set; }

    public virtual RegionZone Region { get; set; } = null!;
}
