using RealEstate.Domain.Entities;

namespace RealEstate.TestData;

public static class TestLeads
{
    public static Lead Create(
        Guid propertyId,
        string fullName = "Lead Test",
        string? email = "lead.test@example.com",
        string? phoneNumber = null,
        string message = "Please contact me",
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        Guid? id = null)
    {
        var now = DateTime.UtcNow;

        var leadId = id ?? Guid.NewGuid();
        var created = createdAt ?? now;
        var updated = updatedAt ?? created;

        return new Lead
        {
            Id = leadId,
            PropertyId = propertyId,
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            Message = message,
            CreatedAt = created,
            UpdatedAt = updated,
        };
    }
}