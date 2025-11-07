using DataAccessLogic;
using Domain.DataTables;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Logic;

public interface IRegionService
{
    Task<RegionDto> CreateAsync(RegionCreateDto dto, CancellationToken ct = default);
    Task<RegionDto> UpdateAsync(long regionId, RegionCreateDto dto, CancellationToken ct = default);
}

public class RegionService : IRegionService
{
    private readonly AppDataAccess _db;

    public RegionService(AppDataAccess db) => _db = db;

    public async Task<RegionDto> CreateAsync(RegionCreateDto dto, CancellationToken ct = default)
    {
        var name = (dto.RegionName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Region name is required", nameof(dto.RegionName));

        var entity = new RegionZone
        {
            RegionName = name,
            Description = dto.Description,
            CreatedByUserId = dto.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };
        await _db.RegionZones.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);

        return new RegionDto
        {
            RegionId = entity.RegionId,
            RegionName = entity.RegionName,
            Description = entity.Description,
            Created = (entity.CreatedDate ?? DateTime.UtcNow).ToLocalTime().ToString("dd MMM yyyy HH:mm")
        };
    }

    public async Task<RegionDto> UpdateAsync(long regionId, RegionCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _db.RegionZones.FirstOrDefaultAsync(r => r.RegionId == regionId, ct)
                     ?? throw new KeyNotFoundException("Region not found");
        var name = (dto.RegionName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Region name is required", nameof(dto.RegionName));

        entity.RegionName = name;
        entity.Description = dto.Description;
        await _db.SaveChangesAsync(ct);

        return new RegionDto
        {
            RegionId = entity.RegionId,
            RegionName = entity.RegionName,
            Description = entity.Description,
            Created = (entity.CreatedDate ?? DateTime.UtcNow).ToLocalTime().ToString("dd MMM yyyy HH:mm")
        };
    }
}
