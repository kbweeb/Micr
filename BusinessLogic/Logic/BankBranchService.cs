using Domain.ViewModels.BankBranches;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IBankBranchService
{
    Task<List<BankBranchDto>> GetIndexAsync(CancellationToken ct = default);
    Task<BankBranchDto> CreateAsync(BankBranchFormViewModel form, CancellationToken ct = default);
    Task<BankBranchDto> UpdateAsync(long bankBranchId, BankBranchFormViewModel form, CancellationToken ct = default);
}

public class BankBranchService : IBankBranchService
{
    private readonly MicrDbContext _context;

    public BankBranchService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankBranchDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.BankBranches
            .Include(b => b.Bank)
            .AsNoTracking()
            .OrderBy(b => b.Bank.BankName)
            .ThenBy(b => b.BankBranchName)
            .Select(b => new BankBranchDto
            {
                BankBranchId = b.BankBranchId,
                BankBranchName = b.BankBranchName,
                BankId = b.BankId,
                BankName = b.Bank.BankName,
                IsEnabled = b.IsEnabled,
                Created = (b.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy")
            })
            .ToListAsync(ct);
    }

    public async Task<BankBranchDto> CreateAsync(BankBranchFormViewModel form, CancellationToken ct = default)
    {
        var entity = new MicrDbChequeProcessingSystem.Models.BankBranch
        {
            BankBranchName = form.BankBranchName.Trim(),
            BankId = form.BankId,
            IsEnabled = form.IsEnabled,
            CreatedByUserId = form.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        _context.BankBranches.Add(entity);
        await _context.SaveChangesAsync(ct);

        var bank = await _context.Banks.FindAsync(new object[] { form.BankId }, ct);

        return new BankBranchDto
        {
            BankBranchId = entity.BankBranchId,
            BankBranchName = entity.BankBranchName,
            BankId = entity.BankId,
            BankName = bank?.BankName,
            IsEnabled = entity.IsEnabled,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? ""
        };
    }

    public async Task<BankBranchDto> UpdateAsync(long bankBranchId, BankBranchFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.BankBranches
            .Include(b => b.Bank)
            .FirstOrDefaultAsync(b => b.BankBranchId == bankBranchId, ct)
            ?? throw new InvalidOperationException("Bank branch not found");

        entity.BankBranchName = form.BankBranchName.Trim();
        entity.BankId = form.BankId;
        entity.IsEnabled = form.IsEnabled;

        await _context.SaveChangesAsync(ct);

        return new BankBranchDto
        {
            BankBranchId = entity.BankBranchId,
            BankBranchName = entity.BankBranchName,
            BankId = entity.BankId,
            BankName = entity.Bank.BankName,
            IsEnabled = entity.IsEnabled,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? ""
        };
    }
}
