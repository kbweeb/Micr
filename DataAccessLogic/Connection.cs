using Domain.ApplicationSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLogic;

public static class Connection
{
    public static string GetConnectionString(IConfiguration config, string? name = null)
    {
        var key = string.IsNullOrWhiteSpace(name) ? AppConstants.DefaultConnectionName : name;
        return config.GetConnectionString(key) ?? string.Empty;
    }
}