using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLogic;

public static class DataAccessRegistration
{
    public static IServiceCollection AddDataAccessLogic(
        this IServiceCollection services,
        IConfiguration configuration,
        string? connectionName = null)
    {
        var conn = Connection.GetConnectionString(configuration, connectionName);
        if (!string.IsNullOrWhiteSpace(conn))
        {
            services.AddDbContext<AppDataAccess>(o => o.UseSqlServer(conn));
        }

        services.AddAutoMapper(cfg => cfg.AddProfile<MappingSetup>());
        services.AddScoped(typeof(IGenericDataAccess<>), typeof(GenericDataAccess<>));
        return services;
    }
}