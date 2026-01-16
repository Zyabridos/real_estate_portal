using AutoMapper;
using RealEstate.Application.DTOs.Brokers;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping;

public sealed class BrokersMappingProfile : Profile
{
    public BrokersMappingProfile()
    {
        CreateMap<Broker, BrokerListItemDto>()
            .ForMember(d => d.BrokerId, o => o.MapFrom(s => s.Id));

        CreateMap<Broker, BrokerDetailsDto>()
            .ForMember(d => d.BrokerId, o => o.MapFrom(s => s.Id));
    }
}