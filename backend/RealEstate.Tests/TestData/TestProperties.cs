using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.TestData;

public static class TestProperties
{
    private static readonly Guid DefaultBrokerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public static Property Create(
        Guid? id = null,
        string title = "Test Property Titile",
        string city = "Trondheim",
        decimal price = 4_500_000m,
        PropertyType type = PropertyType.Apartment,
        PropertyStatus status = PropertyStatus.Active,
        DateTime? createdAt = null,
        Guid? brokerId = null)
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
            BrokerId = brokerId ?? DefaultBrokerId,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    public static Property CreateForBroker(
        Guid brokerId,
        Guid? id = null,
        string title = "Broker Property",
        string city = "Trondheim",
        decimal price = 4_500_000m,
        PropertyType type = PropertyType.Apartment,
        PropertyStatus status = PropertyStatus.Active,
        DateTime? createdAt = null)
    {
        return Create(
            id: id,
            title: title,
            city: city,
            price: price,
            type: type,
            status: status,
            createdAt: createdAt,
            brokerId: brokerId
        );
    }
}