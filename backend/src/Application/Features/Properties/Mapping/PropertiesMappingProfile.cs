using AutoMapper;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Properties.Mapping;

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