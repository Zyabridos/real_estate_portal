using AutoMapper;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Properties.Mapping;

public sealed class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<Property, PropertyDetailsDto>();

        CreateMap<Property, PropertyListItemDto>();

        CreateMap<CreatePropertyRequest, Property>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AgencyId, opt => opt.Ignore())
            .ForMember(d => d.BrokerId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdatePropertyRequest, Property>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AgencyId, opt => opt.Ignore())
            .ForMember(d => d.BrokerId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());
    }
}
