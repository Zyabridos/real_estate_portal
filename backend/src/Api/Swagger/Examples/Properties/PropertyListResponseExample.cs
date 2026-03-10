using RealEstate.Application.Common;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Domain.Enums.Properties;
using Swashbuckle.AspNetCore.Filters;

namespace RealEstate.Api.Swagger.Examples.Properties;

public sealed class PropertyListResponseExample : IExamplesProvider<PagedResult<PropertyListItemDto>>
{
    public PagedResult<PropertyListItemDto> GetExamples()
    {
        return new PagedResult<PropertyListItemDto>
        {
            Items =
            [
                new PropertyListItemDto(
                    Id: 123,
                    AgencyId: 1,
                    BrokerId: 1001,
                    Title: "Modern apartment in city center",
                    City: "Trondheim",
                    Price: 4_500_000m,
                    Type: PropertyType.Apartment,
                    Status: PropertyStatus.Active,
                    MainImageUrl: "https://example.com/image.jpg",
                    CreatedAt: DateTime.SpecifyKind(
                        DateTime.Parse("2026-01-01T10:00:00Z"),
                        DateTimeKind.Utc)
                ),
                new PropertyListItemDto(
                    Id: 123,
                    AgencyId: 2,
                    BrokerId: 1021,
                    Title: "Modern house in city center",
                    City: "Trondheim",
                    Price: 4_500_000m,
                    Type: PropertyType.House,
                    Status: PropertyStatus.Active,
                    MainImageUrl: "https://example.com/image.jpg",
                    CreatedAt: DateTime.SpecifyKind(
                        DateTime.Parse("2026-01-01T10:00:00Z"),
                        DateTimeKind.Utc)
                )
            ],
            TotalItems = 2,
            Page = 1,
            PageSize = 20
        };
    }
}