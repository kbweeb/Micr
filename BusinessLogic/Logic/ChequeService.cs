using AutoMapper;
using DataAccessLogic;
using Domain.DataTables;
using Domain.DTOs;

namespace BusinessLogic.Logic;

public interface IChequeService
{
    Task<ChequeDto> CreateAsync(ChequeDto dto, CancellationToken ct = default);
}

public class ChequeService : IChequeService
{
    private readonly IGenericDataAccess<Cheque> _repo;
    private readonly IMapper _mapper;

    public ChequeService(IGenericDataAccess<Cheque> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ChequeDto> CreateAsync(ChequeDto dto, CancellationToken ct = default)
    {
        // simple validation
        if (string.IsNullOrWhiteSpace(dto.Number))
            throw new ArgumentException("Cheque number is required", nameof(dto.Number));
        if (dto.Amount < 0)
            throw new ArgumentException("Amount must be >= 0", nameof(dto.Amount));

        var entity = _mapper.Map<Cheque>(dto);
        entity.Number = dto.Number?.Trim();
        await _repo.AddAsync(entity, ct);
        return _mapper.Map<ChequeDto>(entity);
    }
}