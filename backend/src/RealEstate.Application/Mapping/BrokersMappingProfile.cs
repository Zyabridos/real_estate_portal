using AutoMapper;
using RealEstate.Application.DTOs.Brokers;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping;

public sealed class BrokersMappingProfile : Profile
{
    public BrokersMappingProfile()
    {
        CreateMap<Broker, BrokerListItemDto>();
        CreateMap<Broker, BrokerDetailsDto>();
    }
}