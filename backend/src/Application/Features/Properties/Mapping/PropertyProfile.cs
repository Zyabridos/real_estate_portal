using AutoMapper;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Properties.Mapping;

public sealed class PropertyProfile : Profile
{
    public PropertyProfile()
    {
        CreateMap<Property, PropertyListItemDto>();
        CreateMap<Property, PropertyDetailsDto>();
    }
}