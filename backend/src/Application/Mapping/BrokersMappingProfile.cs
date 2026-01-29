using AutoMapper;
using RealEstate.Application.DTOs.Brokers;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping;

public sealed class BrokersMappingProfile : Profile
{
    public BrokersMappingProfile()
    {
        // Entity -> DTO
        CreateMap<Broker, BrokerDetailsDto>()
            .ConstructUsing(b => new BrokerDetailsDto(
                b.Id,
                b.AgencyId,
                b.FirstName,
                b.LastName,
                b.Email,
                b.PhoneNumber,
                b.PhotoUrl,
                b.CreatedAt,
                b.UpdatedAt
            ));

        CreateMap<Broker, BrokerListItemDto>()
            .ConstructUsing(b => new BrokerListItemDto(
                b.Id,
                b.FirstName,
                b.LastName,
                b.Email,
                b.PhoneNumber,
                b.PhotoUrl,
                b.CreatedAt,
                b.UpdatedAt
            ));

        // DTO -> Entity
        CreateMap<CreateBrokerRequest, Broker>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdateBrokerRequest, Broker>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());
    }
}