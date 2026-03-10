using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Brokers.List;

namespace RealEstate.Api.Swagger.Examples.Brokers;

public sealed class BrokerListResponseExample : IExamplesProvider<PagedResult<BrokerListItemDto>>
{
    public PagedResult<BrokerListItemDto> GetExamples()
    {
        return new PagedResult<BrokerListItemDto>
        {
            Items =
            [
                new BrokerListItemDto(
                    Id: 1000,
                    FirstName: "Ola",
                    LastName: "Nordmann",
                    Email: "ola.nordmann@realestate.no",
                    PhoneNumber: "+47 111 11 111",
                    PhotoUrl: "https://cdn.example.com/brokers/ola.jpg",
                    CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
                    UpdatedAt: DateTime.Parse("2026-01-10T13:00:00Z")
                ),
                new BrokerListItemDto(
                    Id: 1001,
                    FirstName: "Kari",
                    LastName: "Nordmann",
                    Email: "kari.nordmann@realestate.no",
                    PhoneNumber: "+47 222 22 222",
                    PhotoUrl: null,
                    CreatedAt: DateTime.Parse("2026-01-11T09:30:00Z"),
                    UpdatedAt: DateTime.Parse("2026-01-11T10:30:00Z")
                )
            ],
            TotalItems = 2,
            Page = 1,
            PageSize = 20
        };
    }
}