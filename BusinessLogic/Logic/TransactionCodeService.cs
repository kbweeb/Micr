using Domain.ViewModels.TransactionCodes;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface ITransactionCodeService
{
    Task<List<TransactionCodeDto>> GetIndexAsync(CancellationToken ct = default);
    Task<TransactionCodeDto> CreateAsync(TransactionCodeFormViewModel form, CancellationToken ct = default);
    Task<TransactionCodeDto> UpdateAsync(long transactionCodeId, TransactionCodeFormViewModel form, CancellationToken ct = default);
}

public class TransactionCodeService : ITransactionCodeService
{
    private readonly MicrDbContext _context;

    public TransactionCodeService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionCodeDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.TransactionCodes
            .Include(t => t.CreatedByUser)
            .AsNoTracking()
            .OrderBy(t => t.Code)
            .Select(t => new TransactionCodeDto
            {
                TransactionCodeId = t.TransactionCodeId,
                Code = t.Code,
                Created = t.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = t.CreatedByUser.Fullname
            })
            .ToListAsync(ct);
    }

    public async Task<TransactionCodeDto> CreateAsync(TransactionCodeFormViewModel form, CancellationToken ct = default)
    {
        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.TransactionCode
        {
            Code = form.Code.Trim(),
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.TransactionCodes.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new TransactionCodeDto
        {
            TransactionCodeId = entity.TransactionCodeId,
            Code = entity.Code,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<TransactionCodeDto> UpdateAsync(long transactionCodeId, TransactionCodeFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.TransactionCodes.FirstOrDefaultAsync(t => t.TransactionCodeId == transactionCodeId, ct)
            ?? throw new InvalidOperationException("Transaction code not found");

        entity.Code = form.Code.Trim();

        await _context.SaveChangesAsync(ct);

        return new TransactionCodeDto
        {
            TransactionCodeId = entity.TransactionCodeId,
            Code = entity.Code,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
