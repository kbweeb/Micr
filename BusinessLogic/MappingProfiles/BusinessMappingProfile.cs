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
    }
}