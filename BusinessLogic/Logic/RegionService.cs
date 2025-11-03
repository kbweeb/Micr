using AutoMapper;
using DataAccessLogic;
using Domain.DataTables;
using Domain.DTOs;

namespace BusinessLogic.Logic;

public interface IRegionService
{
    Task<RegionDto> CreateAsync(RegionCreateDto dto, CancellationToken ct = default);
}

public class RegionService : IRegionService
{
    private readonly IGenericDataAccess<RegionZone> _repo;
    private readonly IMapper _mapper;

    public RegionService(IGenericDataAccess<RegionZone> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<RegionDto> CreateAsync(RegionCreateDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<RegionZone>(dto);
        entity.RegionName = dto.RegionName.Trim();
        entity.CreatedDate = DateTime.UtcNow;
        await _repo.AddAsync(entity, ct);
        return _mapper.Map<RegionDto>(entity);
    }
}
