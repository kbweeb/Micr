using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class AccountType
{
    public long AccountTypeId { get; set; }

    public string AccountTypeCode { get; set; } = null!;

    public string AccountTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<BookType> BookTypes { get; set; } = new List<BookType>();

    public virtual UserProfile CreatedByUser { get; set; } = null!;
}
