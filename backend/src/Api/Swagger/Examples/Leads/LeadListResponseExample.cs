using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Api.Swagger.Examples.Leads;

public sealed class LeadListResponseExample : IExamplesProvider<PagedResult<LeadListItemDto>>
{
    public PagedResult<LeadListItemDto> GetExamples()
    {
        return new PagedResult<LeadListItemDto>
        {
            Items =
            [
                new LeadListItemDto(
                    Id: 1,
                    AgencyId: 1,
                    BrokerId: 1002,
                    PropertyId: 123,
                    FullName: "Rob Stark",
                    Email: "rob.stark@winterfale.com",
                    PhoneNumber: "+47 111 22 333",
                    Status: LeadStatus.New,
                    CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
                    UpdatedAt: DateTime.Parse("2026-01-11T13:00:00Z")
                ),
                new LeadListItemDto(
                    Id: 2,
                    AgencyId: 2,
                    BrokerId: 1008,
                    PropertyId: 124,
                    FullName: "Arya Stark",
                    Email: "arya.stark@winterfale.com",
                    PhoneNumber: "+47 111 22 333",
                    Status: LeadStatus.New,
                    CreatedAt: DateTime.Parse("2026-01-10T14:00:00Z"),
                    UpdatedAt: DateTime.Parse("2026-01-11T15:00:00Z")
                )
            ],
            TotalItems = 2,
            Page = 1,
            PageSize = 20
        };
    }
}