using System.Threading;
using RealEstate.Domain.Entities;

namespace RealEstate.TestData;

public static class TestAgencies
{
    private static int _agencyId = 1000;

    public static Agency Create(
        int? id = null,
        string name = "Test Agency",
        string orgNumber = "123123123121",
        string phoneNumber = "+4744444444",
        string city = "Oslo",
        string street = "Test Street 12",
        string zipCode = "1234",
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        var now = DateTime.UtcNow;
        var created = createdAt ?? now;
        var updated = updatedAt ?? created;

        return new Agency
        {
            Id = id ?? Interlocked.Increment(ref _agencyId),
            Name = name,
            OrgNumber = orgNumber,
            PhoneNumber = phoneNumber,
            City = city,
            Street = street,
            ZipCode = zipCode,
            CreatedAt = created,
            UpdatedAt = updated,
        };
    }
}