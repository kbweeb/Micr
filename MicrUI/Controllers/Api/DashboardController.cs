using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly MicrDbContext _context;

    public DashboardController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var banksTask = _context.Banks.CountAsync(cancellationToken);
        var branchesTask = _context.BankBranches.CountAsync(cancellationToken);
        var customersTask = _context.CustomerProfiles.CountAsync(cancellationToken);
        var regionsTask = _context.RegionZones.CountAsync(cancellationToken);

        await Task.WhenAll(banksTask, branchesTask, customersTask, regionsTask);

        return Ok(new
        {
            Banks = banksTask.Result,
            Branches = branchesTask.Result,
            Customers = customersTask.Result,
            Regions = regionsTask.Result
        });
    }

    [HttpGet("regions")]
    public async Task<IActionResult> GetRegionBreakdown(CancellationToken cancellationToken)
    {
        var payload = await _context.RegionZones
            .AsNoTracking()
            .Select(r => new
            {
                r.RegionId,
                Name = r.RegionName,
                Banks = r.Banks.Count,
                Branches = r.Banks.SelectMany(b => b.BankBranches).Count()
            })
            .OrderByDescending(r => r.Branches)
            .ToListAsync(cancellationToken);

        return Ok(payload);
    }
}
