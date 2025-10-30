using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class NumberOfLeaflet
{
    public long NumberOfLeafletId { get; set; }

    public string NumberOfLeaflet1 { get; set; } = null!;

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<BookType> BookTypes { get; set; } = new List<BookType>();

    public virtual UserProfile CreatedByUser { get; set; } = null!;
}
