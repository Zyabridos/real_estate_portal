using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Leads;
using RealEstate.Domain.Enums;

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
                    Id: Guid.Parse("22222222-1111-1111-1111-222222222222"),
                    PropertyId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    FullName: "Rob Stark",
                    Email: "rob.stark@winterfale.com",
                    PhoneNumber: "+47 111 22 333",
                    Status: LeadStatus.New,
                    CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
                    UpdatedAt: DateTime.Parse("2026-01-11T13:00:00Z")
                ),
                new LeadListItemDto(
                    Id: Guid.Parse("33333333-1111-1111-1111-333333331111"),
                    PropertyId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    FullName: "Arya Stark",
                    Email: "arya.stark@winterfale.com",
                    PhoneNumber: "+47 111 22 333",
                    Status: LeadStatus.New,
                    CreatedAt: DateTime.Parse("2026-01-10T14:00:00Z"),
                    UpdatedAt: DateTime.Parse("2026-01-11T15:00:00Z")
                )
            ],
            TotalCount = 2,
            Page = 1,
            PageSize = 20
        };
    }
}