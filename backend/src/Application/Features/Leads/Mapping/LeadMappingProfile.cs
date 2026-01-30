using AutoMapper;
using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mapping.Leads;

public sealed class LeadMappingProfile : Profile
{
    public LeadMappingProfile()
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