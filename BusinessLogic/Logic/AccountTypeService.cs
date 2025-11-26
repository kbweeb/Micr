using Domain.ViewModels.AccountTypes;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IAccountTypeService
{
    Task<List<AccountTypeDto>> GetIndexAsync(CancellationToken ct = default);
    Task<AccountTypeDto> CreateAsync(AccountTypeFormViewModel form, CancellationToken ct = default);
    Task<AccountTypeDto> UpdateAsync(long accountTypeId, AccountTypeFormViewModel form, CancellationToken ct = default);
}

public class AccountTypeService : IAccountTypeService
{
    private readonly MicrDbContext _context;

    public AccountTypeService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountTypeDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.AccountTypes
            .AsNoTracking()
            .OrderBy(a => a.AccountTypeName)
            .Select(a => new AccountTypeDto
            {
                AccountTypeId = a.AccountTypeId,
                AccountTypeName = a.AccountTypeName,
                AccountTypeCode = a.AccountTypeCode,
                Description = a.Description,
                Created = a.CreatedDate.ToString("dd MMM yyyy")
            })
            .ToListAsync(ct);
    }

    public async Task<AccountTypeDto> CreateAsync(AccountTypeFormViewModel form, CancellationToken ct = default)
    {
        var name = form.AccountTypeName.Trim();
        var baseCode = new string(name.Where(char.IsLetterOrDigit).Take(10).Select(char.ToUpper).ToArray());
        if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "ACCTYPE";
        var code = baseCode;
        int i = 1;
        while (await _context.AccountTypes.AnyAsync(a => a.AccountTypeCode == code, ct))
        {
            var attempt = baseCode + i.ToString();
            code = attempt.Length > 10 ? attempt[..10] : attempt;
            i++;
        }

        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.AccountType
        {
            AccountTypeName = name,
            AccountTypeCode = code,
            Description = form.Description?.Trim(),
            IsActive = true,
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.AccountTypes.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new AccountTypeDto
        {
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = entity.AccountTypeName,
            AccountTypeCode = entity.AccountTypeCode,
            Description = entity.Description,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<AccountTypeDto> UpdateAsync(long accountTypeId, AccountTypeFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.AccountTypes.FirstOrDefaultAsync(a => a.AccountTypeId == accountTypeId, ct)
            ?? throw new InvalidOperationException("Account type not found");

        entity.AccountTypeName = form.AccountTypeName.Trim();
        entity.Description = form.Description?.Trim();

        await _context.SaveChangesAsync(ct);

        return new AccountTypeDto
        {
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = entity.AccountTypeName,
            AccountTypeCode = entity.AccountTypeCode,
            Description = entity.Description,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
