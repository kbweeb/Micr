using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MicrDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            var databaseName = $"MicrDbChequeProcessingSystem_Tests_{Guid.NewGuid():N}";
            services.AddDbContext<MicrDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MicrDbContext>();

            context.Database.EnsureCreated();
            DatabaseSeeder.Seed(context);
        });
    }
}
