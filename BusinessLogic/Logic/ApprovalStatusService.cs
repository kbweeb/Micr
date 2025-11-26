using Domain.ViewModels.ApprovalStatuses;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IApprovalStatusService
{
    Task<List<ApprovalStatusDto>> GetIndexAsync(CancellationToken ct = default);
    Task<ApprovalStatusDto> CreateAsync(ApprovalStatusFormViewModel form, CancellationToken ct = default);
    Task<ApprovalStatusDto> UpdateAsync(long approvalStatusId, ApprovalStatusFormViewModel form, CancellationToken ct = default);
}

public class ApprovalStatusService : IApprovalStatusService
{
    private readonly MicrDbContext _context;

    public ApprovalStatusService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApprovalStatusDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.ApprovalStatuses
            .Include(a => a.CreatedByUser)
            .AsNoTracking()
            .OrderBy(a => a.ApprovalStatusName)
            .Select(a => new ApprovalStatusDto
            {
                ApprovalStatusId = a.ApprovalStatusId,
                ApprovalStatusName = a.ApprovalStatusName,
                Created = a.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = a.CreatedByUser.Fullname
            })
            .ToListAsync(ct);
    }

    public async Task<ApprovalStatusDto> CreateAsync(ApprovalStatusFormViewModel form, CancellationToken ct = default)
    {
        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.ApprovalStatus
        {
            ApprovalStatusName = form.ApprovalStatusName.Trim(),
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.ApprovalStatuses.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new ApprovalStatusDto
        {
            ApprovalStatusId = entity.ApprovalStatusId,
            ApprovalStatusName = entity.ApprovalStatusName,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<ApprovalStatusDto> UpdateAsync(long approvalStatusId, ApprovalStatusFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.ApprovalStatuses.FirstOrDefaultAsync(a => a.ApprovalStatusId == approvalStatusId, ct)
            ?? throw new InvalidOperationException("Approval status not found");

        entity.ApprovalStatusName = form.ApprovalStatusName.Trim();

        await _context.SaveChangesAsync(ct);

        return new ApprovalStatusDto
        {
            ApprovalStatusId = entity.ApprovalStatusId,
            ApprovalStatusName = entity.ApprovalStatusName,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
