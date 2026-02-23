using RealEstate.Domain.Entities;

namespace RealEstate.TestData;

public static class TestAgencies
{
    public static Agency Create(
        Guid? id = null,
        string name = "Test Agency",
        string orgNumber = "123123123121",
        string? phoneNumber = "+4744444444",
        string? city = "Oslo",
        string? street = "Test Street 12",
        string zipcode = "1234",
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        var now = DateTime.UtcNow;

        var validId = id ?? Guid.NewGuid();
        var created = createdAt ?? now;
        var updated = updatedAt ?? created;

        return new Agency
        {
            Id = validId,
            Name = name,
            OrgNumber = orgNumber,
            PhoneNumber = phoneNumber,
            City = city,
            Street = street,
            ZipCode = zipcode,
            CreatedAt = created,
            UpdatedAt = updated,
        };
    }
}