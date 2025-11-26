using Domain.ViewModels.Currencies;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface ICurrencyService
{
    Task<List<CurrencyDto>> GetIndexAsync(CancellationToken ct = default);
    Task<CurrencyDto> CreateAsync(CurrencyFormViewModel form, CancellationToken ct = default);
    Task<CurrencyDto> UpdateAsync(long currencyId, CurrencyFormViewModel form, CancellationToken ct = default);
}

public class CurrencyService : ICurrencyService
{
    private readonly MicrDbContext _context;

    public CurrencyService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<CurrencyDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.Currencies
            .Include(c => c.CreatedByUser)
            .AsNoTracking()
            .OrderBy(c => c.CurrencyName)
            .Select(c => new CurrencyDto
            {
                CurrencyId = c.CurrencyId,
                CurrencyName = c.CurrencyName,
                CurrencyCode = c.CurrencyCode,
                Symbol = c.Symbol,
                IsActive = c.IsActive,
                Created = c.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = c.CreatedByUser.Fullname
            })
            .ToListAsync(ct);
    }

    public async Task<CurrencyDto> CreateAsync(CurrencyFormViewModel form, CancellationToken ct = default)
    {
        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.Currency
        {
            CurrencyName = form.CurrencyName.Trim(),
            CurrencyCode = form.CurrencyCode?.Trim(),
            Symbol = form.Symbol?.Trim(),
            IsActive = true,
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.Currencies.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new CurrencyDto
        {
            CurrencyId = entity.CurrencyId,
            CurrencyName = entity.CurrencyName,
            CurrencyCode = entity.CurrencyCode,
            Symbol = entity.Symbol,
            IsActive = entity.IsActive,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<CurrencyDto> UpdateAsync(long currencyId, CurrencyFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyId == currencyId, ct)
            ?? throw new InvalidOperationException("Currency not found");

        entity.CurrencyName = form.CurrencyName.Trim();
        entity.CurrencyCode = form.CurrencyCode?.Trim();
        entity.Symbol = form.Symbol?.Trim();

        await _context.SaveChangesAsync(ct);

        return new CurrencyDto
        {
            CurrencyId = entity.CurrencyId,
            CurrencyName = entity.CurrencyName,
            CurrencyCode = entity.CurrencyCode,
            Symbol = entity.Symbol,
            IsActive = entity.IsActive,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
