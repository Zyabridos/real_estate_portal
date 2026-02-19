using AutoMapper;
using RealEstate.Application.Features.Agency.Create;
using RealEstate.Application.Features.Agency.GetById;
using RealEstate.Application.Features.Agency.List;
using RealEstate.Application.Features.Agency.Update;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Agency.Mapping;

public sealed class AgencyMappingProfile : Profile
{
    public AgencyMappingProfile()
    {
        // Entity -> DTO
        CreateMap<Agency, AgencyDetailsDto>()
            .ConstructUsing(b => new AgencyDetailsDto(
                a.Id,
                a.Name,
                a.OrgNumber,
                a.PhoneNumber,
                a.City,
                a.Street,
                a.ZipCode,
                a.CreatedAt,
                a.UpdatedAt
            ));

        CreateMap<Agency, AgencyListItemDto>()
            .ConstructUsing(b => new AgencyListItemDto(
                a.Id,
                a.Name,
                a.OrgNumber,
                a.PhoneNumber,
                a.City,
                a.Street,
                a.ZipCode,
                a.CreatedAt,
                a.UpdatedAt
            ));

        // DTO -> Entity
        CreateMap<CreateAgencyRequest, Agency>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdateAgencyRequest, Agency>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());
    }
}