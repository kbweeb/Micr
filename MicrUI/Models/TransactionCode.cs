using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class TransactionCode
{
    public long TransactionCodeId { get; set; }

    public string Code { get; set; } = null!;

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<BookType> BookTypes { get; set; } = new List<BookType>();

    public virtual UserProfile CreatedByUser { get; set; } = null!;
}
