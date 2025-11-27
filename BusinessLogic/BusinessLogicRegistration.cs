using BusinessLogic.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class BusinessLogicRegistration
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        // Register the single centralized portal service
        // All controllers should depend ONLY on IMicrPortalService
        services.AddScoped<IMicrPortalService, MicrPortalService>();

        // Register IHttpContextAccessor for user context access
        services.AddHttpContextAccessor();

        return services;
    }
}
