using Domain.DataTables;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic;

public class AppDataAccess : DbContext
{
    public AppDataAccess(DbContextOptions<AppDataAccess> options) : base(options) { }

    public DbSet<Cheque> Cheques => Set<Cheque>();
    public DbSet<Customer> Customers => Set<Customer>();
}