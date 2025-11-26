using Domain.ViewModels.Statuses;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IStatusService
{
    Task<List<StatusDto>> GetIndexAsync(CancellationToken ct = default);
    Task<StatusDto> CreateAsync(StatusFormViewModel form, CancellationToken ct = default);
    Task<StatusDto> UpdateAsync(long statusId, StatusFormViewModel form, CancellationToken ct = default);
}

public class StatusService : IStatusService
{
    private readonly MicrDbContext _context;

    public StatusService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<StatusDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.Statuses
            .Include(s => s.CreatedByUser)
            .AsNoTracking()
            .OrderBy(s => s.StatusName)
            .Select(s => new StatusDto
            {
                StatusId = s.StatusId,
                StatusName = s.StatusName,
                Created = s.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = s.CreatedByUser.Fullname
            })
            .ToListAsync(ct);
    }

    public async Task<StatusDto> CreateAsync(StatusFormViewModel form, CancellationToken ct = default)
    {
        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.Status
        {
            StatusName = form.StatusName.Trim(),
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.Statuses.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new StatusDto
        {
            StatusId = entity.StatusId,
            StatusName = entity.StatusName,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<StatusDto> UpdateAsync(long statusId, StatusFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.Statuses.FirstOrDefaultAsync(s => s.StatusId == statusId, ct)
            ?? throw new InvalidOperationException("Status not found");

        entity.StatusName = form.StatusName.Trim();

        await _context.SaveChangesAsync(ct);

        return new StatusDto
        {
            StatusId = entity.StatusId,
            StatusName = entity.StatusName,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
