using MicrDbChequeProcessingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic;

public class AppDataAccess : DbContext
{
    public AppDataAccess(DbContextOptions<AppDataAccess> options) : base(options) { }

    public DbSet<AccountType> AccountTypes => Set<AccountType>();
    public DbSet<RegionZone> RegionZones => Set<RegionZone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}