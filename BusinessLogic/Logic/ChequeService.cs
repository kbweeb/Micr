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
        var entity = _mapper.Map<Cheque>(dto);
        await _repo.AddAsync(entity, ct);
        return _mapper.Map<ChequeDto>(entity);
    }
}