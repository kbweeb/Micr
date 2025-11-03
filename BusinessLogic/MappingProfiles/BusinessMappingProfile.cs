using AutoMapper;
using Domain.DataTables;
using Domain.DTOs;
using Domain.ViewModels;

namespace BusinessLogic.MappingProfiles;

public class BusinessMappingProfile : Profile
{
    public BusinessMappingProfile()
    {
        CreateMap<Cheque, ChequeDto>().ReverseMap();
        CreateMap<ChequeDto, ChequeViewModel>();

        CreateMap<Domain.DTOs.AccountTypeCreateDto, Domain.DataTables.AccountType>();
        CreateMap<Domain.DataTables.AccountType, Domain.DTOs.AccountTypeDto>()
            .ForMember(d => d.AccountTypeId, o => o.MapFrom(s => s.AccountTypeId))
            .ForMember(d => d.AccountTypeName, o => o.MapFrom(s => s.AccountTypeName))
            .ForMember(d => d.AccountTypeCode, o => o.MapFrom(s => s.AccountTypeCode))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            .ForMember(d => d.Created, o => o.MapFrom(s => s.CreatedDate.ToLocalTime().ToString("dd MMM yyyy HH:mm")));

        CreateMap<Domain.DTOs.RegionCreateDto, Domain.DataTables.RegionZone>();
        CreateMap<Domain.DataTables.RegionZone, Domain.DTOs.RegionDto>()
            .ForMember(d => d.RegionId, o => o.MapFrom(s => s.RegionId))
            .ForMember(d => d.RegionName, o => o.MapFrom(s => s.RegionName))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            .ForMember(d => d.Created, o => o.MapFrom(s => (s.CreatedDate ?? DateTime.UtcNow).ToLocalTime().ToString("dd MMM yyyy HH:mm")));
    }
}