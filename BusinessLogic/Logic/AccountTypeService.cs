using AutoMapper;
using DataAccessLogic;
using Domain.DataTables;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Logic;

public interface IAccountTypeService
{
    Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto dto, CancellationToken ct = default);
}

public class AccountTypeService : IAccountTypeService
{
    private readonly IGenericDataAccess<AccountType> _repo;
    private readonly IMapper _mapper;

    public AccountTypeService(IGenericDataAccess<AccountType> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<AccountTypeDto> CreateAsync(AccountTypeCreateDto dto, CancellationToken ct = default)
    {
        var name = dto.AccountTypeName.Trim();
        var baseCode = new string(name.Where(char.IsLetterOrDigit).Take(10).Select(char.ToUpper).ToArray());
        if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "ACCTYPE";
        var code = baseCode;
        int i = 1;
        while (await _repo.Queryable.AnyAsync(a => a.AccountTypeCode == code, ct))
        {
            var attempt = baseCode + i.ToString();
            code = attempt.Length > 10 ? attempt[..10] : attempt;
            i++;
        }

        var entity = _mapper.Map<AccountType>(dto);
        entity.AccountTypeCode = code;
        entity.AccountTypeName = name;
        entity.IsActive = true;
        entity.CreatedDate = DateTime.UtcNow;

        await _repo.AddAsync(entity, ct);
        return _mapper.Map<AccountTypeDto>(entity);
    }
}
