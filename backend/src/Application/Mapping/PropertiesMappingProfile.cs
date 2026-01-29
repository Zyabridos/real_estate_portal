using AutoMapper;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping;

public sealed class PropertiesMappingProfile : Profile
{
    public PropertiesMappingProfile()
    {
        CreateMap<Property, PropertyListItemDto>()
            .ForCtorParam("Type", opt => opt.MapFrom(x => x.Type.ToString()))
            .ForCtorParam("Status", opt => opt.MapFrom(x => x.Status.ToString()));

        CreateMap<Property, PropertyDetailsDto>()
            .ForCtorParam("Type", opt => opt.MapFrom(x => x.Type.ToString()))
            .ForCtorParam("Status", opt => opt.MapFrom(x => x.Status.ToString()));
    }
}