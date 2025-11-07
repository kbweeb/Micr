using DataAccessLogic;
using Domain.DataTables;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Logic;

public interface IAccountTypeService
{
    Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto dto, CancellationToken ct = default);
    Task<AccountTypeDto> UpdateAsync(long accountTypeId, AccountTypeCreateDto dto, CancellationToken ct = default);
}

public class AccountTypeService : IAccountTypeService
{
    private readonly AppDataAccess _db;

    public AccountTypeService(AppDataAccess db) => _db = db;

    public async Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto dto, CancellationToken ct = default)
    {
        var name = (dto.AccountTypeName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account type name is required", nameof(dto.AccountTypeName));

        var baseCode = new string(name.Where(char.IsLetterOrDigit).Take(10).Select(char.ToUpper).ToArray());
        if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "ACCTYPE";
        var code = baseCode;
        int i = 1;
        while (await _db.AccountTypes.AnyAsync(a => a.AccountTypeCode == code, ct))
        {
            var attempt = baseCode + i.ToString();
            code = attempt.Length > 10 ? attempt[..10] : attempt;
            i++;
        }

        var entity = new AccountType
        {
            AccountTypeCode = code,
            AccountTypeName = name,
            Description = dto.Description,
            IsActive = true,
            CreatedByUserId = dto.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        await _db.AccountTypes.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);

        return new AccountTypeDto
        {
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = entity.AccountTypeName,
            AccountTypeCode = entity.AccountTypeCode,
            Description = entity.Description,
            Created = entity.CreatedDate.ToLocalTime().ToString("dd MMM yyyy HH:mm")
        };
    }

    public async Task<AccountTypeDto> UpdateAsync(long accountTypeId, AccountTypeCreateDto dto, CancellationToken ct = default)
    {
        var entity = await _db.AccountTypes.FirstOrDefaultAsync(a => a.AccountTypeId == accountTypeId, ct)
                     ?? throw new KeyNotFoundException("Account type not found");

        var name = (dto.AccountTypeName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account type name is required", nameof(dto.AccountTypeName));

        entity.AccountTypeName = name;
        entity.Description = dto.Description;

        await _db.SaveChangesAsync(ct);

        return new AccountTypeDto
        {
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = entity.AccountTypeName,
            AccountTypeCode = entity.AccountTypeCode,
            Description = entity.Description,
            Created = entity.CreatedDate.ToLocalTime().ToString("dd MMM yyyy HH:mm")
        };
    }
}
