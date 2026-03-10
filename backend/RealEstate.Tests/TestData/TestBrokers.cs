using System.Threading;
using RealEstate.Domain.Entities;

namespace RealEstate.TestData;

public static class TestBrokers
{
    private static int _brokerId = 1000;

    public static Broker Create(
        int? id = null,
        int agencyId = 1,
        string firstName = "Alice",
        string lastName = "Agent",
        string? email = null,
        string phoneNumber = "+4744444444",
        string? photoUrl = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        var now = DateTime.UtcNow;

        var brokerId = id ?? Interlocked.Increment(ref _brokerId);
        var created = createdAt ?? now;
        var updated = updatedAt ?? created;

        var safeEmail = email ?? CreateUniqueEmail(firstName, lastName);

        return new Broker
        {
            Id = brokerId,
            AgencyId = agencyId,
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