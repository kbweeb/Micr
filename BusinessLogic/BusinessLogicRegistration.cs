using AutoMapper;
using BusinessLogic.Logic;
using BusinessLogic.MappingProfiles;
using BusinessLogic.Validation;
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

        // Validators (registered for DI usage in services/controllers)
        services.AddScoped<FluentValidation.IValidator<Domain.DTOs.ChequeDto>, ChequeValidator>();

        return services;
    }
}
