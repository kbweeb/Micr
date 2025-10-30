using System;
using System.Collections.Generic;

namespace MicrDbChequeProcessingSystem.Models;

public partial class UserProfile
{
    public long UserId { get; set; }

    public long BankId { get; set; }

    public long BankBranchId { get; set; }

    public string Fullname { get; set; } = null!;

    public string? Surname { get; set; }

    public string? Firstname { get; set; }

    public string? Othername { get; set; }

    public string Username { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public bool? IsEnabled { get; set; }

    public int? NoOfTrials { get; set; }

    public int? PasswordUpdateInterval { get; set; }

    public DateTime? LastPasswordUpdateDate { get; set; }

    public long CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long ApprovedStatusId { get; set; }

    public long? ApprovedUserId { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public virtual ICollection<AccountType> AccountTypes { get; set; } = new List<AccountType>();

    public virtual ICollection<ApprovalStatus> ApprovalStatuses { get; set; } = new List<ApprovalStatus>();

    public virtual ApprovalStatus ApprovedStatus { get; set; } = null!;

    public virtual UserProfile? ApprovedUser { get; set; }

    public virtual ICollection<BankBranch> BankBranches { get; set; } = new List<BankBranch>();

    public virtual ICollection<Bank> Banks { get; set; } = new List<Bank>();

    public virtual ICollection<BookType> BookTypes { get; set; } = new List<BookType>();

    public virtual UserProfile CreatedByUser { get; set; } = null!;

    public virtual ICollection<Currency> Currencies { get; set; } = new List<Currency>();

    public virtual ICollection<CustomerProfile> CustomerProfiles { get; set; } = new List<CustomerProfile>();

    public virtual ICollection<UserProfile> InverseApprovedUser { get; set; } = new List<UserProfile>();

    public virtual ICollection<UserProfile> InverseCreatedByUser { get; set; } = new List<UserProfile>();

    public virtual ICollection<NumberOfLeaflet> NumberOfLeaflets { get; set; } = new List<NumberOfLeaflet>();

    public virtual ICollection<RegionZone> RegionZones { get; set; } = new List<RegionZone>();

    public virtual ICollection<Status> Statuses { get; set; } = new List<Status>();

    public virtual ICollection<TransactionCode> TransactionCodes { get; set; } = new List<TransactionCode>();
}
