using RealEstate.Domain.Entities;

namespace RealEstate.TestData;

public static class TestBrokers
{
    public static Broker Create(
        string firstName = "Alice",
        string lastName = "Agent",
        string? email = null,
        string phoneNumber = "+4744444444",
        string? photoUrl = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        Guid? id = null)
    {
        var now = DateTime.UtcNow;

        var brokerId = id ?? Guid.NewGuid();
        var created = createdAt ?? now;
        var updated = updatedAt ?? created;

        var safeEmail = email ?? CreateUniqueEmail(firstName, lastName);

        return new Broker
        {
            Id = brokerId,
            FirstName = firstName,
            LastName = lastName,
            Email = safeEmail,
            PhoneNumber = phoneNumber,
            PhotoUrl = photoUrl,
            CreatedAt = created,
            UpdatedAt = updated,
        };
    }

    private static string CreateUniqueEmail(string firstName, string lastName)
    {
        var stamp = Guid.NewGuid().ToString("N")[..10];
        return $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}.{stamp}@example.com";
    }
}