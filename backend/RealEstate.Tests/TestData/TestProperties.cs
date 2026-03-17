using System.Threading;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Properties;

namespace RealEstate.TestData;

public static class TestProperties
{
    private static int _propertyId = 1000;

    public static Property Create(
        int? id = null,
        int agencyId = 1,
        int brokerId = 1,
        string title = "Test Property Title",
        string city = "Trondheim",
        decimal price = 4_500_000m,
        PropertyType type = PropertyType.Apartment,
        PropertyStatus status = PropertyStatus.Active,
        DateTime? createdAt = null)
    {
        return new Property
        {
            Id = id ?? Interlocked.Increment(ref _propertyId),
            AgencyId = agencyId,
            BrokerId = brokerId,
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
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    public static Property CreateForBroker(
        int brokerId,
        int agencyId = 1,
        int? id = null,
        string title = "Broker Property",
        string city = "Trondheim",
        decimal price = 4_500_000m,
        PropertyType type = PropertyType.Apartment,
        PropertyStatus status = PropertyStatus.Active,
        DateTime? createdAt = null)
    {
        return Create(
            id: id,
            agencyId: agencyId,
            brokerId: brokerId,
            title: title,
            city: city,
            price: price,
            type: type,
            status: status,
            createdAt: createdAt
        );
    }
}