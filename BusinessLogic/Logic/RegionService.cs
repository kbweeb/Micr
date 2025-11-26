using Domain.ViewModels.Regions;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IRegionService
{
    Task<List<RegionDto>> GetIndexAsync(CancellationToken ct = default);
    Task<RegionDto> CreateAsync(RegionFormViewModel form, CancellationToken ct = default);
    Task<RegionDto> UpdateAsync(long regionId, RegionFormViewModel form, CancellationToken ct = default);
}

public class RegionService : IRegionService
{
    private readonly MicrDbContext _context;

    public RegionService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<RegionDto>> GetIndexAsync(CancellationToken ct = default)
    {
        var regions = await _context.RegionZones
            .Include(r => r.Banks)
                .ThenInclude(b => b.BankBranches)
            .AsNoTracking()
            .OrderBy(r => r.RegionName)
            .ToListAsync(ct);

        return regions.Select(r => new RegionDto
        {
            RegionId = r.RegionId,
            RegionName = r.RegionName,
            Description = r.Description,
            Created = (r.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy"),
            Banks = r.Banks.Count,
            Branches = r.Banks.Sum(b => b.BankBranches.Count)
        }).ToList();
    }

    public async Task<RegionDto> CreateAsync(RegionFormViewModel form, CancellationToken ct = default)
    {
        var entity = new MicrDbChequeProcessingSystem.Models.RegionZone
        {
            RegionName = form.RegionName.Trim(),
            Description = form.Description?.Trim(),
            CreatedByUserId = form.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        _context.RegionZones.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new RegionDto
        {
            RegionId = entity.RegionId,
            RegionName = entity.RegionName,
            Description = entity.Description,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? "",
            Banks = 0,
            Branches = 0
        };
    }

    public async Task<RegionDto> UpdateAsync(long regionId, RegionFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.RegionZones
            .Include(r => r.Banks)
                .ThenInclude(b => b.BankBranches)
            .FirstOrDefaultAsync(r => r.RegionId == regionId, ct)
            ?? throw new InvalidOperationException("Region not found");

        entity.RegionName = form.RegionName.Trim();
        entity.Description = form.Description?.Trim();

        await _context.SaveChangesAsync(ct);

        return new RegionDto
        {
            RegionId = entity.RegionId,
            RegionName = entity.RegionName,
            Description = entity.Description,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? "",
            Banks = entity.Banks.Count,
            Branches = entity.Banks.Sum(b => b.BankBranches.Count)
        };
    }
}
