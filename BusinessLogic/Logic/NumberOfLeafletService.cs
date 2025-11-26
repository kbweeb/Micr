using Domain.ViewModels.NumberOfLeaflets;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface INumberOfLeafletService
{
    Task<List<NumberOfLeafletDto>> GetIndexAsync(CancellationToken ct = default);
    Task<NumberOfLeafletDto> CreateAsync(NumberOfLeafletFormViewModel form, CancellationToken ct = default);
    Task<NumberOfLeafletDto> UpdateAsync(long numberOfLeafletId, NumberOfLeafletFormViewModel form, CancellationToken ct = default);
}

public class NumberOfLeafletService : INumberOfLeafletService
{
    private readonly MicrDbContext _context;

    public NumberOfLeafletService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<NumberOfLeafletDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.NumberOfLeaflets
            .Include(n => n.CreatedByUser)
            .AsNoTracking()
            .OrderBy(n => n.NumberOfLeaflet1)
            .Select(n => new NumberOfLeafletDto
            {
                NumberOfLeafletId = n.NumberOfLeafletId,
                NumberOfLeaflet = n.NumberOfLeaflet1,
                Created = n.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = n.CreatedByUser.Fullname
            })
            .ToListAsync(ct);
    }

    public async Task<NumberOfLeafletDto> CreateAsync(NumberOfLeafletFormViewModel form, CancellationToken ct = default)
    {
        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.NumberOfLeaflet
        {
            NumberOfLeaflet1 = form.NumberOfLeaflet,
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.NumberOfLeaflets.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new NumberOfLeafletDto
        {
            NumberOfLeafletId = entity.NumberOfLeafletId,
            NumberOfLeaflet = entity.NumberOfLeaflet1,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<NumberOfLeafletDto> UpdateAsync(long numberOfLeafletId, NumberOfLeafletFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.NumberOfLeaflets.FirstOrDefaultAsync(n => n.NumberOfLeafletId == numberOfLeafletId, ct)
            ?? throw new InvalidOperationException("Number of leaflet not found");

        entity.NumberOfLeaflet1 = form.NumberOfLeaflet;

        await _context.SaveChangesAsync(ct);

        return new NumberOfLeafletDto
        {
            NumberOfLeafletId = entity.NumberOfLeafletId,
            NumberOfLeaflet = entity.NumberOfLeaflet1,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
