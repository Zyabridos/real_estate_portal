using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Domain.Enums.Properties;
using Swashbuckle.AspNetCore.Filters;

namespace RealEstate.Api.Swagger.Examples.Properties;

public sealed class PropertyDetailsResponseExample : IExamplesProvider<PropertyDetailsDto>
{
    public PropertyDetailsDto GetExamples()
    {
        return new PropertyDetailsDto(
            Id: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Title: "Spacious family house",
            Description: "A bright house with a large garden.",
            Address: "Example street 10",
            City: "Trondheim",
            Price: 7_900_000m,
            Type: PropertyType.House,
            Bedrooms: 4,
            Bathrooms: 2,
            Area: 180m,
            Status: PropertyStatus.Active,
            MainImageUrl: "https://example.com/house.jpg",
            BrokerId: Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            CreatedAt: DateTime.SpecifyKind(
                DateTime.Parse("2025-12-15T12:00:00Z"),
                DateTimeKind.Utc)
        );
    }
}