using AutoMapper;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping;

public sealed class PropertyProfile : Profile
{
    public PropertyProfile()
    {
        CreateMap<Property, PropertyListItemDto>();
        CreateMap<Property, PropertyDetailsDto>();
    }
}