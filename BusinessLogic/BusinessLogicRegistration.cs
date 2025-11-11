using AutoMapper;
using BusinessLogic.Logic;
using BusinessLogic.MappingProfiles;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class BusinessLogicRegistration
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<BusinessMappingProfile>());
        services.AddScoped<IChequeService, ChequeService>();
        services.AddScoped<IAccountTypeService, AccountTypeService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IApplicationLogic, ApplicationLogic>();

        return services;
    }
}
