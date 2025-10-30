using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class BankBranch
{
    public long BankBranchId { get; set; }

    public string BankBranchName { get; set; } = null!;

    public long BankId { get; set; }

    public bool IsEnabled { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Bank Bank { get; set; } = null!;

    public virtual UserProfile? CreatedByUser { get; set; }

    public virtual ICollection<CustomerProfile> CustomerProfiles { get; set; } = new List<CustomerProfile>();
}
