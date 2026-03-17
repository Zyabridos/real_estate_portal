using AutoMapper;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;
using RealEstate.Domain.Entities;
using Application.Features.Properties.Common;

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
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(
            dest => dest.ImageUrls,
            opt => opt.MapFrom(src =>
                PropertyImageUrlsNormalizer.Normalize(src.MainImageUrl, src.ImageUrls)));


        CreateMap<UpdatePropertyRequest, Property>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AgencyId, opt => opt.Ignore())
            .ForMember(d => d.BrokerId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());
    }
}
