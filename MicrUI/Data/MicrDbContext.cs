using System;
using System.Collections.Generic;
using MicrDbChequeProcessingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MicrDbChequeProcessingSystem.Data;

public partial class MicrDbContext : DbContext
{
    public MicrDbContext()
    {
    }

    public MicrDbContext(DbContextOptions<MicrDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountType> AccountTypes { get; set; }
    public virtual DbSet<ApprovalStatus> ApprovalStatuses { get; set; }
    public virtual DbSet<Bank> Banks { get; set; }
    public virtual DbSet<BankBranch> BankBranches { get; set; }
    public virtual DbSet<BookType> BookTypes { get; set; }
    public virtual DbSet<Currency> Currencies { get; set; }
    public virtual DbSet<CustomerProfile> CustomerProfiles { get; set; }
    public virtual DbSet<NumberOfLeaflet> NumberOfLeaflets { get; set; }
    public virtual DbSet<RegionZone> RegionZones { get; set; }
    public virtual DbSet<Status> Statuses { get; set; }
    public virtual DbSet<TransactionCode> TransactionCodes { get; set; }
    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    // ✅ Removed the OnConfiguring hardcoded connection string
    // The configuration will now come from Program.cs (appsettings.json)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.ToTable("AccountType");

            entity.Property(e => e.AccountTypeCode).HasMaxLength(10);
            entity.Property(e => e.AccountTypeName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.AccountTypes)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountType_UserProfile");
        });

        modelBuilder.Entity<ApprovalStatus>(entity =>
        {
            entity.ToTable("ApprovalStatus");

            entity.Property(e => e.ApprovalStatusName).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ApprovalStatuses)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApprovalStatus_UserProfile");
        });

        // Description support on AccountType
        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(400);
        });

        // Description support on Region(Zone)
        modelBuilder.Entity<RegionZone>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(400);
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.ToTable("Bank");

            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsEnabled).HasColumnName("Is_Enabled");
            entity.Property(e => e.SortCode).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Banks)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_Bank_UserProfile");

            entity.HasOne(d => d.Region).WithMany(p => p.Banks)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bank_Region(Zone)");
        });

        modelBuilder.Entity<BankBranch>(entity =>
        {
            entity.ToTable("BankBranch");

            entity.Property(e => e.BankBranchName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsEnabled).HasColumnName("Is_Enabled");

            entity.HasOne(d => d.Bank).WithMany(p => p.BankBranches)
                .HasForeignKey(d => d.BankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BankBranch_Bank");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.BankBranches)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_BankBranch_UserProfile");
        });

        modelBuilder.Entity<BookType>(entity =>
        {
            entity.ToTable("BookType");

            entity.Property(e => e.BookTypeCode).HasMaxLength(10);
            entity.Property(e => e.BookTypeName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.AccountType).WithMany(p => p.BookTypes)
                .HasForeignKey(d => d.AccountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookType_AccountType");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.BookTypes)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookType_UserProfile");

            entity.HasOne(d => d.NumberOfLeaflet).WithMany(p => p.BookTypes)
                .HasForeignKey(d => d.NumberOfLeafletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookType_NumberOfLeaflet");

            entity.HasOne(d => d.TransactionCode).WithMany(p => p.BookTypes)
                .HasForeignKey(d => d.TransactionCodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookType_TransactionCode");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currency");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CurrencyName).HasMaxLength(50);
            entity.Property(e => e.Symbol).HasMaxLength(10);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Currencies)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Currency_UserProfile");
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.ToTable("CustomerProfile");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(50);
            entity.Property(e => e.T24customerName)
                .HasMaxLength(150)
                .HasColumnName("T24CustomerName");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CustomerProfiles)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_CustomerProfile_UserProfile");

            entity.HasOne(d => d.RequestingBankBranch).WithMany(p => p.CustomerProfiles)
                .HasForeignKey(d => d.RequestingBankBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerProfile_BankBranch");
        });

        modelBuilder.Entity<NumberOfLeaflet>(entity =>
        {
            entity.ToTable("NumberOfLeaflet");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.NumberOfLeaflet1)
                .HasMaxLength(100)
                .HasColumnName("NumberOfLeaflet");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.NumberOfLeaflets)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NumberOfLeaflet_UserProfile");
        });

        modelBuilder.Entity<RegionZone>(entity =>
        {
            entity.HasKey(e => e.RegionId);

            entity.ToTable("Region(Zone)");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.RegionName).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.RegionZones)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_Region(Zone)_UserProfile");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusName).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Statuses)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Status_UserProfile");
        });

        modelBuilder.Entity<TransactionCode>(entity =>
        {
            entity.ToTable("TransactionCode");

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.TransactionCodes)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionCode_UserProfile");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserProfile");

            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailAddress).HasMaxLength(100);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Fullname).HasMaxLength(150);
            entity.Property(e => e.IsEnabled).HasColumnName("Is_Enabled");
            entity.Property(e => e.LastPasswordUpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("Last_Password_Update_Date");
            entity.Property(e => e.NoOfTrials).HasColumnName("No_Of_Trials");
            entity.Property(e => e.Othername).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.PasswordUpdateInterval).HasColumnName("Password_Update_Interval");
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.ApprovedStatus).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.ApprovedStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_ApprovalStatus");

            entity.HasOne(d => d.ApprovedUser).WithMany(p => p.InverseApprovedUser)
                .HasForeignKey(d => d.ApprovedUserId)
                .HasConstraintName("FK_UserProfile_UserProfile1");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.InverseCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_UserProfile");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
