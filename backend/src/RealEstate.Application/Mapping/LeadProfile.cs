using AutoMapper;
using RealEstate.Application.DTOs.Leads;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping.Leads;

public sealed class LeadProfile : Profile
{
    public LeadProfile()
    {
        // Domain -> DTO
        CreateMap<Lead, LeadListItemDto>();
        CreateMap<Lead, LeadDetailsDto>();

        // Request DTO -> Domain
        CreateMap<CreateLeadRequest, Lead>();

        // Update DTO -> Domain
        CreateMap<UpdateLeadRequest, Lead>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember is not null));
    }
}