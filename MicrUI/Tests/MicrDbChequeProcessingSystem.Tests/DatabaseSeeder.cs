using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Tests;

internal static class DatabaseSeeder
{
    public static void Seed(MicrDbContext context)
    {
        if (context.RegionZones.Any())
        {
            return;
        }

        var region = new RegionZone
        {
            RegionId = 1,
            RegionName = "Greater Accra",
            CreatedDate = DateTime.UtcNow
        };

        var bank = new Bank
        {
            BankId = 1,
            BankName = "Apex Test Bank",
            SortCode = "123456",
            Region = region,
            IsEnabled = true,
            CreatedDate = DateTime.UtcNow
        };

        var branch = new BankBranch
        {
            BankBranchId = 1,
            BankBranchName = "Apex Branch",
            Bank = bank,
            IsEnabled = true,
            CreatedDate = DateTime.UtcNow
        };

        var customer = new CustomerProfile
        {
            CustomerId = 1,
            CustomerName = "John Doe",
            AccountNumber = "0001112223",
            RequestingBankBranch = branch,
            CreatedDate = DateTime.UtcNow
        };

        context.RegionZones.Add(region);
        context.Banks.Add(bank);
        context.BankBranches.Add(branch);
        context.CustomerProfiles.Add(customer);

        context.SaveChanges();
    }
}
