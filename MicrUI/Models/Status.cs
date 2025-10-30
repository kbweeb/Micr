using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class Status
{
    public long StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual UserProfile CreatedByUser { get; set; } = null!;
}
