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
                    Id: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Title: "Modern apartment in city center",
                    City: "Trondheim",
                    Price: 4_500_000m,
                    Type: PropertyType.Apartment,
                    Status: PropertyStatus.Active,
                    MainImageUrl: "https://example.com/image.jpg",
                    CreatedAt: DateTime.SpecifyKind(
                        DateTime.Parse("2026-01-01T10:00:00Z"),
                        DateTimeKind.Utc)
                )
            ],
            TotalItems = 1,
            Page = 1,
            PageSize = 10
        };
    }
}