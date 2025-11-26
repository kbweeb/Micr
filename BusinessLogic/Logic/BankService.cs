using Domain.ViewModels.Banks;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IBankService
{
    Task<List<BankDto>> GetIndexAsync(CancellationToken ct = default);
    Task<BankDto> CreateAsync(BankFormViewModel form, CancellationToken ct = default);
    Task<BankDto> UpdateAsync(long bankId, BankFormViewModel form, CancellationToken ct = default);
}

public class BankService : IBankService
{
    private readonly MicrDbContext _context;

    public BankService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.Banks
            .Include(b => b.Region)
            .AsNoTracking()
            .OrderBy(b => b.BankName)
            .Select(b => new BankDto
            {
                BankId = b.BankId,
                BankName = b.BankName,
                SortCode = b.SortCode,
                RegionId = b.RegionId,
                RegionName = b.Region.RegionName,
                IsEnabled = b.IsEnabled ?? false,
                Created = (b.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy")
            })
            .ToListAsync(ct);
    }

    public async Task<BankDto> CreateAsync(BankFormViewModel form, CancellationToken ct = default)
    {
        var entity = new MicrDbChequeProcessingSystem.Models.Bank
        {
            BankName = form.BankName.Trim(),
            SortCode = form.SortCode.Trim(),
            RegionId = form.RegionId,
            IsEnabled = form.IsEnabled,
            CreatedByUserId = form.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        _context.Banks.Add(entity);
        await _context.SaveChangesAsync(ct);

        var region = await _context.RegionZones.FindAsync(new object[] { form.RegionId }, ct);

        return new BankDto
        {
            BankId = entity.BankId,
            BankName = entity.BankName,
            SortCode = entity.SortCode,
            RegionId = entity.RegionId,
            RegionName = region?.RegionName,
            IsEnabled = entity.IsEnabled ?? false,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? ""
        };
    }

    public async Task<BankDto> UpdateAsync(long bankId, BankFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.Banks
            .Include(b => b.Region)
            .FirstOrDefaultAsync(b => b.BankId == bankId, ct)
            ?? throw new InvalidOperationException("Bank not found");

        entity.BankName = form.BankName.Trim();
        entity.SortCode = form.SortCode.Trim();
        entity.RegionId = form.RegionId;
        entity.IsEnabled = form.IsEnabled;

        await _context.SaveChangesAsync(ct);

        return new BankDto
        {
            BankId = entity.BankId,
            BankName = entity.BankName,
            SortCode = entity.SortCode,
            RegionId = entity.RegionId,
            RegionName = entity.Region.RegionName,
            IsEnabled = entity.IsEnabled ?? false,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? ""
        };
    }
}
