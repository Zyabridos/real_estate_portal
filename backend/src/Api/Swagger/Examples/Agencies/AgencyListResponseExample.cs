using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Agencies.List;

namespace RealEstate.Api.Swagger.Examples.Agencies;

public sealed class AgencyListResponseExample : IExamplesProvider<PagedResult<AgencyListItemDto>>
{
    public PagedResult<AgencyListItemDto> GetExamples()
    {
        return new PagedResult<AgencyListItemDto>
        {
            Items =
            [
                new AgencyListItemDto(
                    Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name: "Eiendommenbyrå #1",
                    OrgNumber: "1234567891011",
                    PhoneNumber: "+47 111 11 111",
                    City: "Oslo",
                    Street: "Torggata 123",
                    ZipCode: "1020",
                    CreatedAt: DateTime.Parse("2024-01-10T12:00:00Z"),
                    UpdatedAt: DateTime.Parse("2024-01-10T13:00:00Z")
                ),
                new AgencyListItemDto(
                    Id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name: "Eiendommenbyrå #1 i Trondheim",
                    OrgNumber: "1112223334441",
                    PhoneNumber: "+47 222 22 222",
                    City: "Trondheim",
                    Street: "Jonsvannsveien 123",
                    ZipCode: "7071",
                    CreatedAt: DateTime.Parse("2025-01-11T09:30:00Z"),
                    UpdatedAt: DateTime.Parse("2025-01-11T10:30:00Z")
                )
            ],
            TotalItems = 2,
            Page = 1,
            PageSize = 20
        };
    }
}