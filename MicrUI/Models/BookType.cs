using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class BookType
{
    public long BookTypeId { get; set; }

    public string BookTypeCode { get; set; } = null!;

    public string BookTypeName { get; set; } = null!;

    public long AccountTypeId { get; set; }

    public long TransactionCodeId { get; set; }

    public long NumberOfLeafletId { get; set; }

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual AccountType AccountType { get; set; } = null!;

    public virtual UserProfile CreatedByUser { get; set; } = null!;

    public virtual NumberOfLeaflet NumberOfLeaflet { get; set; } = null!;

    public virtual TransactionCode TransactionCode { get; set; } = null!;
}
