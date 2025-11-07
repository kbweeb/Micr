using BusinessLogic.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class BusinessLogicRegistration
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IChequeService, ChequeService>();
        services.AddScoped<IAccountTypeService, AccountTypeService>();
        services.AddScoped<IRegionService, RegionService>();

        return services;
    }
}
