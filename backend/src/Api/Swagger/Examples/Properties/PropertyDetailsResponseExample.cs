using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Domain.Enums.Properties;
using Swashbuckle.AspNetCore.Filters;

namespace RealEstate.Api.Swagger.Examples.Properties;

public sealed class PropertyDetailsResponseExample : IExamplesProvider<PropertyDetailsDto>
{
    public PropertyDetailsDto GetExamples()
    {
        return new PropertyDetailsDto(
            Id: 123,
            AgencyId: 1,
            BrokerId: 1001,
            Title: "Spacious family house",
            Description: "A bright house with a large garden.",
            Address: "Example street 10",
            City: "Trondheim",
            Price: 7_900_000m,
            Bedrooms: 4,
            Bathrooms: 2,
            Area: 180m,
            MainImageUrl: "https://example.com/house.jpg",
            ImageUrls:
            [
                "https://example.com/kitchen.jpg",
                "https://example.com/bathroom.jpg",
                "https://example.com/second_bathroom.jpg",
            ],
            Status: PropertyStatus.Active,
            Type: PropertyType.House,
            CreatedAt: DateTime.Parse("2025-12-15T12:00:00Z"),
            UpdatedAt: DateTime.Parse("2026-01-11T12:00:00Z")
        );
    }
}