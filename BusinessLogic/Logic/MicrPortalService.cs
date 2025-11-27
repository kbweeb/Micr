using Domain.ViewModels.AccountTypes;
using Domain.ViewModels.ApprovalStatuses;
using Domain.ViewModels.BankBranches;
using Domain.ViewModels.Banks;
using Domain.ViewModels.BookTypes;
using Domain.ViewModels.Currencies;
using Domain.ViewModels.NumberOfLeaflets;
using Domain.ViewModels.Regions;
using Domain.ViewModels.Statuses;
using Domain.ViewModels.TransactionCodes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;

namespace BusinessLogic.Logic;

/// <summary>
/// Central portal service for all MICR Cheque Processing System business logic.
/// All data access and business rules are centralized here.
/// External classes should ONLY access this through the IMicrPortalService interface.
/// </summary>
public class MicrPortalService : IMicrPortalService
{
    private readonly MicrDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MicrPortalService(MicrDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    #region User Context
    public long GetCurrentUserId()
    {
        // Placeholder: In production, extract from claims/session
        // var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        // if (long.TryParse(userIdClaim, out var userId)) return userId;
        return 0; // Return 0 to indicate no logged-in user
    }

    private async Task<long?> GetValidUserIdAsync(long? providedUserId, CancellationToken ct)
    {
        // If a specific userId was provided, verify it exists
        if (providedUserId.HasValue && providedUserId.Value > 0)
        {
            var exists = await _context.UserProfiles.AnyAsync(u => u.UserId == providedUserId.Value, ct);
            if (exists) return providedUserId.Value;
        }

        // Try to get current logged-in user
        var currentUserId = GetCurrentUserId();
        if (currentUserId > 0)
        {
            var exists = await _context.UserProfiles.AnyAsync(u => u.UserId == currentUserId, ct);
            if (exists) return currentUserId;
        }

        // Fallback: get first user from database
        var firstUser = await _context.UserProfiles.FirstOrDefaultAsync(ct);
        if (firstUser != null)
            return firstUser.UserId;

        // No users exist - return null (CreatedByUserId is nullable)
        return null;
    }
    #endregion

    #region Bank Operations
    public async Task<List<BankDto>> GetBanksAsync(CancellationToken ct = default)
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

    public async Task<BankDto?> GetBankByIdAsync(long bankId, CancellationToken ct = default)
    {
        return await _context.Banks
            .Include(b => b.Region)
            .AsNoTracking()
            .Where(b => b.BankId == bankId)
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
            .FirstOrDefaultAsync(ct);
    }

    public async Task<BankDto> CreateBankAsync(BankFormViewModel form, CancellationToken ct = default)
    {
        ValidateBankForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new Bank
        {
            BankName = form.BankName.Trim(),
            SortCode = form.SortCode.Trim(),
            RegionId = form.RegionId,
            IsEnabled = form.IsEnabled,
            CreatedByUserId = userId,
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

    public async Task<BankDto> UpdateBankAsync(long bankId, BankFormViewModel form, CancellationToken ct = default)
    {
        ValidateBankForm(form);

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

    public async Task<bool> DeleteBankAsync(long bankId, CancellationToken ct = default)
    {
        var entity = await _context.Banks.FindAsync(new object[] { bankId }, ct);
        if (entity == null) return false;

        // Soft delete
        entity.IsEnabled = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateBankForm(BankFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.BankName))
            throw new ArgumentException("Bank name is required");
        if (string.IsNullOrWhiteSpace(form.SortCode))
            throw new ArgumentException("Sort code is required");
        if (form.RegionId <= 0)
            throw new ArgumentException("Region is required");
    }
    #endregion

    #region Bank Branch Operations
    public async Task<List<BankBranchDto>> GetBankBranchesAsync(CancellationToken ct = default)
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

    public async Task<List<BankBranchDto>> GetBankBranchesByBankIdAsync(long bankId, CancellationToken ct = default)
    {
        return await _context.BankBranches
            .Include(b => b.Bank)
            .AsNoTracking()
            .Where(b => b.BankId == bankId)
            .OrderBy(b => b.BankBranchName)
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

    public async Task<BankBranchDto?> GetBankBranchByIdAsync(long bankBranchId, CancellationToken ct = default)
    {
        return await _context.BankBranches
            .Include(b => b.Bank)
            .AsNoTracking()
            .Where(b => b.BankBranchId == bankBranchId)
            .Select(b => new BankBranchDto
            {
                BankBranchId = b.BankBranchId,
                BankBranchName = b.BankBranchName,
                BankId = b.BankId,
                BankName = b.Bank.BankName,
                IsEnabled = b.IsEnabled,
                Created = (b.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy")
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<BankBranchDto> CreateBankBranchAsync(BankBranchFormViewModel form, CancellationToken ct = default)
    {
        ValidateBankBranchForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new BankBranch
        {
            BankBranchName = form.BankBranchName.Trim(),
            BankId = form.BankId,
            IsEnabled = form.IsEnabled,
            CreatedByUserId = userId,
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

    public async Task<BankBranchDto> UpdateBankBranchAsync(long bankBranchId, BankBranchFormViewModel form, CancellationToken ct = default)
    {
        ValidateBankBranchForm(form);

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

    public async Task<bool> DeleteBankBranchAsync(long bankBranchId, CancellationToken ct = default)
    {
        var entity = await _context.BankBranches.FindAsync(new object[] { bankBranchId }, ct);
        if (entity == null) return false;

        entity.IsEnabled = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateBankBranchForm(BankBranchFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.BankBranchName))
            throw new ArgumentException("Bank branch name is required");
        if (form.BankId <= 0)
            throw new ArgumentException("Bank is required");
    }
    #endregion

    #region Account Type Operations
    public async Task<List<AccountTypeDto>> GetAccountTypesAsync(CancellationToken ct = default)
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

    public async Task<AccountTypeDto?> GetAccountTypeByIdAsync(long accountTypeId, CancellationToken ct = default)
    {
        return await _context.AccountTypes
            .AsNoTracking()
            .Where(a => a.AccountTypeId == accountTypeId)
            .Select(a => new AccountTypeDto
            {
                AccountTypeId = a.AccountTypeId,
                AccountTypeName = a.AccountTypeName,
                AccountTypeCode = a.AccountTypeCode,
                Description = a.Description,
                Created = a.CreatedDate.ToString("dd MMM yyyy")
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<AccountTypeDto> CreateAccountTypeAsync(AccountTypeFormViewModel form, CancellationToken ct = default)
    {
        ValidateAccountTypeForm(form);

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

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new AccountType
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

    public async Task<AccountTypeDto> UpdateAccountTypeAsync(long accountTypeId, AccountTypeFormViewModel form, CancellationToken ct = default)
    {
        ValidateAccountTypeForm(form);

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

    public async Task<bool> DeleteAccountTypeAsync(long accountTypeId, CancellationToken ct = default)
    {
        var entity = await _context.AccountTypes.FindAsync(new object[] { accountTypeId }, ct);
        if (entity == null) return false;

        entity.IsActive = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateAccountTypeForm(AccountTypeFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.AccountTypeName))
            throw new ArgumentException("Account type name is required");
    }
    #endregion

    #region Region Operations
    public async Task<List<RegionDto>> GetRegionsAsync(CancellationToken ct = default)
    {
        var regions = await _context.RegionZones
            .Include(r => r.Banks)
                .ThenInclude(b => b.BankBranches)
            .AsNoTracking()
            .OrderBy(r => r.RegionName)
            .ToListAsync(ct);

        return regions.Select(r => new RegionDto
        {
            RegionId = r.RegionId,
            RegionName = r.RegionName,
            Description = r.Description,
            Created = (r.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy"),
            Banks = r.Banks.Count,
            Branches = r.Banks.Sum(b => b.BankBranches.Count)
        }).ToList();
    }

    public async Task<RegionDto?> GetRegionByIdAsync(long regionId, CancellationToken ct = default)
    {
        var region = await _context.RegionZones
            .Include(r => r.Banks)
                .ThenInclude(b => b.BankBranches)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RegionId == regionId, ct);

        if (region == null) return null;

        return new RegionDto
        {
            RegionId = region.RegionId,
            RegionName = region.RegionName,
            Description = region.Description,
            Created = (region.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy"),
            Banks = region.Banks.Count,
            Branches = region.Banks.Sum(b => b.BankBranches.Count)
        };
    }

    public async Task<RegionDto> CreateRegionAsync(RegionFormViewModel form, CancellationToken ct = default)
    {
        ValidateRegionForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new RegionZone
        {
            RegionName = form.RegionName.Trim(),
            Description = form.Description?.Trim(),
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.RegionZones.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new RegionDto
        {
            RegionId = entity.RegionId,
            RegionName = entity.RegionName,
            Description = entity.Description,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? "",
            Banks = 0,
            Branches = 0
        };
    }

    public async Task<RegionDto> UpdateRegionAsync(long regionId, RegionFormViewModel form, CancellationToken ct = default)
    {
        ValidateRegionForm(form);

        var entity = await _context.RegionZones
            .Include(r => r.Banks)
                .ThenInclude(b => b.BankBranches)
            .FirstOrDefaultAsync(r => r.RegionId == regionId, ct)
            ?? throw new InvalidOperationException("Region not found");

        entity.RegionName = form.RegionName.Trim();
        entity.Description = form.Description?.Trim();

        await _context.SaveChangesAsync(ct);

        return new RegionDto
        {
            RegionId = entity.RegionId,
            RegionName = entity.RegionName,
            Description = entity.Description,
            Created = entity.CreatedDate?.ToString("dd MMM yyyy") ?? "",
            Banks = entity.Banks.Count,
            Branches = entity.Banks.Sum(b => b.BankBranches.Count)
        };
    }

    public async Task<bool> DeleteRegionAsync(long regionId, CancellationToken ct = default)
    {
        var entity = await _context.RegionZones.FindAsync(new object[] { regionId }, ct);
        if (entity == null) return false;

        _context.RegionZones.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateRegionForm(RegionFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.RegionName))
            throw new ArgumentException("Region name is required");
    }
    #endregion

    #region Currency Operations
    public async Task<List<CurrencyDto>> GetCurrenciesAsync(CancellationToken ct = default)
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
                CreatedBy = c.CreatedByUser != null ? c.CreatedByUser.Fullname : null
            })
            .ToListAsync(ct);
    }

    public async Task<CurrencyDto?> GetCurrencyByIdAsync(long currencyId, CancellationToken ct = default)
    {
        return await _context.Currencies
            .Include(c => c.CreatedByUser)
            .AsNoTracking()
            .Where(c => c.CurrencyId == currencyId)
            .Select(c => new CurrencyDto
            {
                CurrencyId = c.CurrencyId,
                CurrencyName = c.CurrencyName,
                CurrencyCode = c.CurrencyCode,
                Symbol = c.Symbol,
                IsActive = c.IsActive,
                Created = c.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = c.CreatedByUser != null ? c.CreatedByUser.Fullname : null
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CurrencyDto> CreateCurrencyAsync(CurrencyFormViewModel form, CancellationToken ct = default)
    {
        ValidateCurrencyForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new Currency
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

    public async Task<CurrencyDto> UpdateCurrencyAsync(long currencyId, CurrencyFormViewModel form, CancellationToken ct = default)
    {
        ValidateCurrencyForm(form);

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

    public async Task<bool> DeleteCurrencyAsync(long currencyId, CancellationToken ct = default)
    {
        var entity = await _context.Currencies.FindAsync(new object[] { currencyId }, ct);
        if (entity == null) return false;

        entity.IsActive = false;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateCurrencyForm(CurrencyFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.CurrencyName))
            throw new ArgumentException("Currency name is required");
    }
    #endregion

    #region Status Operations
    public async Task<List<StatusDto>> GetStatusesAsync(CancellationToken ct = default)
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
                CreatedBy = s.CreatedByUser != null ? s.CreatedByUser.Fullname : null
            })
            .ToListAsync(ct);
    }

    public async Task<StatusDto?> GetStatusByIdAsync(long statusId, CancellationToken ct = default)
    {
        return await _context.Statuses
            .Include(s => s.CreatedByUser)
            .AsNoTracking()
            .Where(s => s.StatusId == statusId)
            .Select(s => new StatusDto
            {
                StatusId = s.StatusId,
                StatusName = s.StatusName,
                Created = s.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = s.CreatedByUser != null ? s.CreatedByUser.Fullname : null
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<StatusDto> CreateStatusAsync(StatusFormViewModel form, CancellationToken ct = default)
    {
        ValidateStatusForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new Status
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

    public async Task<StatusDto> UpdateStatusAsync(long statusId, StatusFormViewModel form, CancellationToken ct = default)
    {
        ValidateStatusForm(form);

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

    public async Task<bool> DeleteStatusAsync(long statusId, CancellationToken ct = default)
    {
        var entity = await _context.Statuses.FindAsync(new object[] { statusId }, ct);
        if (entity == null) return false;

        _context.Statuses.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateStatusForm(StatusFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.StatusName))
            throw new ArgumentException("Status name is required");
    }
    #endregion

    #region Transaction Code Operations
    public async Task<List<TransactionCodeDto>> GetTransactionCodesAsync(CancellationToken ct = default)
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
                CreatedBy = t.CreatedByUser != null ? t.CreatedByUser.Fullname : null
            })
            .ToListAsync(ct);
    }

    public async Task<TransactionCodeDto?> GetTransactionCodeByIdAsync(long transactionCodeId, CancellationToken ct = default)
    {
        return await _context.TransactionCodes
            .Include(t => t.CreatedByUser)
            .AsNoTracking()
            .Where(t => t.TransactionCodeId == transactionCodeId)
            .Select(t => new TransactionCodeDto
            {
                TransactionCodeId = t.TransactionCodeId,
                Code = t.Code,
                Created = t.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = t.CreatedByUser != null ? t.CreatedByUser.Fullname : null
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<TransactionCodeDto> CreateTransactionCodeAsync(TransactionCodeFormViewModel form, CancellationToken ct = default)
    {
        ValidateTransactionCodeForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new TransactionCode
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

    public async Task<TransactionCodeDto> UpdateTransactionCodeAsync(long transactionCodeId, TransactionCodeFormViewModel form, CancellationToken ct = default)
    {
        ValidateTransactionCodeForm(form);

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

    public async Task<bool> DeleteTransactionCodeAsync(long transactionCodeId, CancellationToken ct = default)
    {
        var entity = await _context.TransactionCodes.FindAsync(new object[] { transactionCodeId }, ct);
        if (entity == null) return false;

        _context.TransactionCodes.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateTransactionCodeForm(TransactionCodeFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.Code))
            throw new ArgumentException("Transaction code is required");
    }
    #endregion

    #region Book Type Operations
    public async Task<List<BookTypeDto>> GetBookTypesAsync(CancellationToken ct = default)
    {
        return await _context.BookTypes
            .Include(b => b.AccountType)
            .Include(b => b.NumberOfLeaflet)
            .Include(b => b.TransactionCode)
            .AsNoTracking()
            .OrderBy(b => b.BookTypeName)
            .Select(b => new BookTypeDto
            {
                BookTypeId = b.BookTypeId,
                BookTypeCode = b.BookTypeCode,
                BookTypeName = b.BookTypeName,
                AccountTypeId = b.AccountTypeId,
                AccountTypeName = b.AccountType.AccountTypeName,
                NumberOfLeafletId = b.NumberOfLeafletId,
                NumberOfLeaflet = b.NumberOfLeaflet.NumberOfLeaflet1,
                TransactionCodeId = b.TransactionCodeId,
                TransactionCode = b.TransactionCode.Code,
                Created = b.CreatedDate.ToString("dd MMM yyyy")
            })
            .ToListAsync(ct);
    }

    public async Task<BookTypeDto?> GetBookTypeByIdAsync(long bookTypeId, CancellationToken ct = default)
    {
        return await _context.BookTypes
            .Include(b => b.AccountType)
            .Include(b => b.NumberOfLeaflet)
            .Include(b => b.TransactionCode)
            .AsNoTracking()
            .Where(b => b.BookTypeId == bookTypeId)
            .Select(b => new BookTypeDto
            {
                BookTypeId = b.BookTypeId,
                BookTypeCode = b.BookTypeCode,
                BookTypeName = b.BookTypeName,
                AccountTypeId = b.AccountTypeId,
                AccountTypeName = b.AccountType.AccountTypeName,
                NumberOfLeafletId = b.NumberOfLeafletId,
                NumberOfLeaflet = b.NumberOfLeaflet.NumberOfLeaflet1,
                TransactionCodeId = b.TransactionCodeId,
                TransactionCode = b.TransactionCode.Code,
                Created = b.CreatedDate.ToString("dd MMM yyyy")
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<BookTypeDto> CreateBookTypeAsync(BookTypeFormViewModel form, CancellationToken ct = default)
    {
        ValidateBookTypeForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new BookType
        {
            BookTypeCode = form.BookTypeCode.Trim(),
            BookTypeName = form.BookTypeName.Trim(),
            AccountTypeId = form.AccountTypeId,
            NumberOfLeafletId = form.NumberOfLeafletId,
            TransactionCodeId = form.TransactionCodeId,
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.BookTypes.Add(entity);
        await _context.SaveChangesAsync(ct);

        var accountType = await _context.AccountTypes.FindAsync(new object[] { form.AccountTypeId }, ct);
        var leaflet = await _context.NumberOfLeaflets.FindAsync(new object[] { form.NumberOfLeafletId }, ct);
        var transCode = await _context.TransactionCodes.FindAsync(new object[] { form.TransactionCodeId }, ct);

        return new BookTypeDto
        {
            BookTypeId = entity.BookTypeId,
            BookTypeCode = entity.BookTypeCode,
            BookTypeName = entity.BookTypeName,
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = accountType?.AccountTypeName,
            NumberOfLeafletId = entity.NumberOfLeafletId,
            NumberOfLeaflet = leaflet?.NumberOfLeaflet1,
            TransactionCodeId = entity.TransactionCodeId,
            TransactionCode = transCode?.Code,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<BookTypeDto> UpdateBookTypeAsync(long bookTypeId, BookTypeFormViewModel form, CancellationToken ct = default)
    {
        ValidateBookTypeForm(form);

        var entity = await _context.BookTypes
            .Include(b => b.AccountType)
            .Include(b => b.NumberOfLeaflet)
            .Include(b => b.TransactionCode)
            .FirstOrDefaultAsync(b => b.BookTypeId == bookTypeId, ct)
            ?? throw new InvalidOperationException("Book type not found");

        entity.BookTypeCode = form.BookTypeCode.Trim();
        entity.BookTypeName = form.BookTypeName.Trim();
        entity.AccountTypeId = form.AccountTypeId;
        entity.NumberOfLeafletId = form.NumberOfLeafletId;
        entity.TransactionCodeId = form.TransactionCodeId;

        await _context.SaveChangesAsync(ct);

        return new BookTypeDto
        {
            BookTypeId = entity.BookTypeId,
            BookTypeCode = entity.BookTypeCode,
            BookTypeName = entity.BookTypeName,
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = entity.AccountType.AccountTypeName,
            NumberOfLeafletId = entity.NumberOfLeafletId,
            NumberOfLeaflet = entity.NumberOfLeaflet.NumberOfLeaflet1,
            TransactionCodeId = entity.TransactionCodeId,
            TransactionCode = entity.TransactionCode.Code,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<bool> DeleteBookTypeAsync(long bookTypeId, CancellationToken ct = default)
    {
        var entity = await _context.BookTypes.FindAsync(new object[] { bookTypeId }, ct);
        if (entity == null) return false;

        _context.BookTypes.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateBookTypeForm(BookTypeFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.BookTypeCode))
            throw new ArgumentException("Book type code is required");
        if (string.IsNullOrWhiteSpace(form.BookTypeName))
            throw new ArgumentException("Book type name is required");
        if (form.AccountTypeId <= 0)
            throw new ArgumentException("Account type is required");
        if (form.NumberOfLeafletId <= 0)
            throw new ArgumentException("Number of leaflet is required");
        if (form.TransactionCodeId <= 0)
            throw new ArgumentException("Transaction code is required");
    }
    #endregion

    #region Number Of Leaflet Operations
    public async Task<List<NumberOfLeafletDto>> GetNumberOfLeafletsAsync(CancellationToken ct = default)
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
                CreatedBy = n.CreatedByUser != null ? n.CreatedByUser.Fullname : null
            })
            .ToListAsync(ct);
    }

    public async Task<NumberOfLeafletDto?> GetNumberOfLeafletByIdAsync(long numberOfLeafletId, CancellationToken ct = default)
    {
        return await _context.NumberOfLeaflets
            .Include(n => n.CreatedByUser)
            .AsNoTracking()
            .Where(n => n.NumberOfLeafletId == numberOfLeafletId)
            .Select(n => new NumberOfLeafletDto
            {
                NumberOfLeafletId = n.NumberOfLeafletId,
                NumberOfLeaflet = n.NumberOfLeaflet1,
                Created = n.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = n.CreatedByUser != null ? n.CreatedByUser.Fullname : null
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<NumberOfLeafletDto> CreateNumberOfLeafletAsync(NumberOfLeafletFormViewModel form, CancellationToken ct = default)
    {
        ValidateNumberOfLeafletForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new NumberOfLeaflet
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

    public async Task<NumberOfLeafletDto> UpdateNumberOfLeafletAsync(long numberOfLeafletId, NumberOfLeafletFormViewModel form, CancellationToken ct = default)
    {
        ValidateNumberOfLeafletForm(form);

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

    public async Task<bool> DeleteNumberOfLeafletAsync(long numberOfLeafletId, CancellationToken ct = default)
    {
        var entity = await _context.NumberOfLeaflets.FindAsync(new object[] { numberOfLeafletId }, ct);
        if (entity == null) return false;

        _context.NumberOfLeaflets.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateNumberOfLeafletForm(NumberOfLeafletFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.NumberOfLeaflet))
            throw new ArgumentException("Number of leaflet is required");
    }
    #endregion

    #region Approval Status Operations
    public async Task<List<ApprovalStatusDto>> GetApprovalStatusesAsync(CancellationToken ct = default)
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
                CreatedBy = a.CreatedByUser != null ? a.CreatedByUser.Fullname : null
            })
            .ToListAsync(ct);
    }

    public async Task<ApprovalStatusDto?> GetApprovalStatusByIdAsync(long approvalStatusId, CancellationToken ct = default)
    {
        return await _context.ApprovalStatuses
            .Include(a => a.CreatedByUser)
            .AsNoTracking()
            .Where(a => a.ApprovalStatusId == approvalStatusId)
            .Select(a => new ApprovalStatusDto
            {
                ApprovalStatusId = a.ApprovalStatusId,
                ApprovalStatusName = a.ApprovalStatusName,
                Created = a.CreatedDate.ToString("dd MMM yyyy"),
                CreatedBy = a.CreatedByUser != null ? a.CreatedByUser.Fullname : null
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ApprovalStatusDto> CreateApprovalStatusAsync(ApprovalStatusFormViewModel form, CancellationToken ct = default)
    {
        ValidateApprovalStatusForm(form);

        var userId = await GetValidUserIdAsync(form.CreatedByUserId, ct);

        var entity = new ApprovalStatus
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

    public async Task<ApprovalStatusDto> UpdateApprovalStatusAsync(long approvalStatusId, ApprovalStatusFormViewModel form, CancellationToken ct = default)
    {
        ValidateApprovalStatusForm(form);

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

    public async Task<bool> DeleteApprovalStatusAsync(long approvalStatusId, CancellationToken ct = default)
    {
        var entity = await _context.ApprovalStatuses.FindAsync(new object[] { approvalStatusId }, ct);
        if (entity == null) return false;

        _context.ApprovalStatuses.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private void ValidateApprovalStatusForm(ApprovalStatusFormViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.ApprovalStatusName))
            throw new ArgumentException("Approval status name is required");
    }
    #endregion
}
