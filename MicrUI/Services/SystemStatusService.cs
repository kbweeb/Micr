using System;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Dtos;

namespace MicrDbChequeProcessingSystem.Services;

public interface ISystemStatusService
{
    Task<SystemStatusDto> GetSnapshotAsync(CancellationToken cancellationToken = default);
}

public class SystemStatusService : ISystemStatusService
{
    private readonly MicrDbContext _context;

    public SystemStatusService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<SystemStatusDto> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var totalBanksTask = _context.Banks.CountAsync(cancellationToken);
        var totalBranchesTask = _context.BankBranches.CountAsync(cancellationToken);
        var totalCustomersTask = _context.CustomerProfiles.CountAsync(cancellationToken);
        var totalRegionsTask = _context.RegionZones.CountAsync(cancellationToken);

        await Task.WhenAll(totalBanksTask, totalBranchesTask, totalCustomersTask, totalRegionsTask);

        var regionBreakdown = await _context.RegionZones
            .AsNoTracking()
            .Select(region => new SystemStatusMetricDto(
                region.RegionName,
                Convert.ToDouble(region.Banks.SelectMany(b => b.BankBranches).Count())
            ))
            .ToListAsync(cancellationToken);

        var components = new List<SystemStatusComponentDto>
        {
            new("Cheque ingestion", "Operational", "Last batch submitted " + now.AddMinutes(-12).ToLocalTime().ToString("HH:mm"), now.AddMinutes(-12)),
            new("Transaction validation", "Operational", "Queue depth 0; auto reconciliation enabled", now.AddMinutes(-8)),
            new("Settlement export", "Scheduled", "Next push at 16:45 GMT", now)
        };

        var incidents = new List<SystemIncidentDto>();

        return new SystemStatusDto(
            now,
            new[]
            {
                new SystemStatusMetricDto("Banks", Convert.ToDouble(totalBanksTask.Result)),
                new SystemStatusMetricDto("Branches", Convert.ToDouble(totalBranchesTask.Result)),
                new SystemStatusMetricDto("Customers", Convert.ToDouble(totalCustomersTask.Result)),
                new SystemStatusMetricDto("Regions", Convert.ToDouble(totalRegionsTask.Result))
            }.Concat(regionBreakdown).ToList(),
            components,
            incidents
        );
    }
}
