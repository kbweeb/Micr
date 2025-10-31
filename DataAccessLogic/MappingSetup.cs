using AutoMapper;
using Domain.DataTables;
using Domain.DTOs;

namespace DataAccessLogic;

public class MappingSetup : Profile
{
    public MappingSetup()
    {
        CreateMap<Cheque, ChequeDto>().ReverseMap();
    }
}