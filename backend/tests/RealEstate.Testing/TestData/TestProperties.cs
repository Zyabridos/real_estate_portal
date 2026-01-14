using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Testing.TestData;

public static class TestProperties
{
    public static Property Create(
        Guid? id = null,
        string title = "Test Property Titile",
        string city = "Trondheim",
        decimal price = 4_500_000m,
        PropertyType type = PropertyType.Apartment,
        PropertyStatus status = PropertyStatus.Active,
        DateTime? createdAt = null)
    {
        return new Property
        {
            Id = id ?? Guid.NewGuid(),
            Title = title,
            Description = "Some description",
            Address = "Example street 1",
            City = city,
            Price = price,
            Type = type,
            Bedrooms = 2,
            Bathrooms = 1,
            Area = 55m,
            Status = status,
            MainImageUrl = "https://example.com/image.jpg",
            BrokerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }
}