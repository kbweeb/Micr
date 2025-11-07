using Domain.DataTables;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic;

public class AppDataAccess : DbContext
{
    public AppDataAccess(DbContextOptions<AppDataAccess> options) : base(options) { }

    public DbSet<Cheque> Cheques => Set<Cheque>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<AccountType> AccountTypes => Set<AccountType>();
    public DbSet<RegionZone> RegionZones => Set<RegionZone>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<BankBranch> BankBranches => Set<BankBranch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map to existing tables used by MicrDbContext
        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.ToTable("AccountType");
            entity.HasKey(e => e.AccountTypeId);
            entity.Property(e => e.AccountTypeCode).HasMaxLength(10);
            entity.Property(e => e.AccountTypeName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<RegionZone>(entity =>
        {
            entity.ToTable("Region(Zone)");
            entity.HasKey(e => e.RegionId);
            entity.Property(e => e.RegionName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.ToTable("Bank");
            entity.HasKey(e => e.BankId);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.SortCode).HasMaxLength(50);
            entity.Property<DateTime?>("CreatedDate").HasColumnType("datetime");
            entity.HasOne(e => e.Region)
                .WithMany(r => r.Banks)
                .HasForeignKey(e => e.RegionId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bank_Region(Zone)");
        });

        modelBuilder.Entity<BankBranch>(entity =>
        {
            entity.ToTable("BankBranch");
            entity.HasKey(e => e.BankBranchId);
            entity.Property(e => e.BankBranchName).HasMaxLength(100);
            entity.Property<DateTime?>("CreatedDate").HasColumnType("datetime");
            entity.HasOne(e => e.Bank)
                .WithMany(b => b.BankBranches)
                .HasForeignKey(e => e.BankId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BankBranch_Bank");
        });

        base.OnModelCreating(modelBuilder);
    }
}