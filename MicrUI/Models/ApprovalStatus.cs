using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class ApprovalStatus
{
    public long ApprovalStatusId { get; set; }

    public string ApprovalStatusName { get; set; } = null!;

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual UserProfile CreatedByUser { get; set; } = null!;

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
