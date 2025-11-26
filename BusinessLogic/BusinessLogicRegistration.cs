using BusinessLogic.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class BusinessLogicRegistration
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IAccountTypeService, AccountTypeService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ITransactionCodeService, TransactionCodeService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IBankBranchService, BankBranchService>();
        services.AddScoped<IBookTypeService, BookTypeService>();
        services.AddScoped<IApprovalStatusService, ApprovalStatusService>();
        services.AddScoped<INumberOfLeafletService, NumberOfLeafletService>();

        return services;
    }
}
