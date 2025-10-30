using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class CustomerProfile
{
    public long CustomerId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string? T24customerName { get; set; }

    public string CustomerName { get; set; } = null!;

    public long RequestingBankBranchId { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual UserProfile? CreatedByUser { get; set; }

    public virtual BankBranch RequestingBankBranch { get; set; } = null!;
}
