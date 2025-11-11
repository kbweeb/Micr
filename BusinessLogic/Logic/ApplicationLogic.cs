using Domain.DTOs;

namespace BusinessLogic.Logic;

public interface IApplicationLogic
{
    // AccountType
    Task<IReadOnlyList<AccountTypeDto>> GetAccountTypesAsync(CancellationToken ct = default);
    Task<AccountTypeDto> CreateAccountTypeAsync(AccountTypeCreateDto dto, CancellationToken ct = default);
    Task<AccountTypeDto> UpdateAccountTypeAsync(long accountTypeId, AccountTypeCreateDto dto, CancellationToken ct = default);

    // Region
    Task<IReadOnlyList<RegionIndexDto>> GetRegionsIndexAsync(CancellationToken ct = default);
    Task<RegionDto> CreateRegionAsync(RegionCreateDto dto, CancellationToken ct = default);
    Task<RegionDto> UpdateRegionAsync(long regionId, RegionCreateDto dto, CancellationToken ct = default);
}

public class ApplicationLogic : IApplicationLogic
{
    private readonly IAccountTypeService _accountTypeService;
    private readonly IRegionService _regionService;

    public ApplicationLogic(
        IAccountTypeService accountTypeService,
        IRegionService regionService)
    {
        _accountTypeService = accountTypeService;
        _regionService = regionService;
    }

    // AccountType
    public async Task<IReadOnlyList<AccountTypeDto>> GetAccountTypesAsync(CancellationToken ct = default)
        => await _accountTypeService.GetAllAsync(ct);

    public async Task<AccountTypeDto> CreateAccountTypeAsync(AccountTypeCreateDto dto, CancellationToken ct = default)
        => await _accountTypeService.CreateAsync(dto, ct);

    public async Task<AccountTypeDto> UpdateAccountTypeAsync(long accountTypeId, AccountTypeCreateDto dto, CancellationToken ct = default)
        => await _accountTypeService.UpdateAsync(accountTypeId, dto, ct);

    // Region
    public async Task<IReadOnlyList<RegionIndexDto>> GetRegionsIndexAsync(CancellationToken ct = default)
        => await _regionService.GetIndexAsync(ct);

    public async Task<RegionDto> CreateRegionAsync(RegionCreateDto dto, CancellationToken ct = default)
        => await _regionService.CreateAsync(dto, ct);

    public async Task<RegionDto> UpdateRegionAsync(long regionId, RegionCreateDto dto, CancellationToken ct = default)
        => await _regionService.UpdateAsync(regionId, dto, ct);
}
