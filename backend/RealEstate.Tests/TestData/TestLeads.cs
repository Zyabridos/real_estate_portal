using System.Threading;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.TestData;

public static class TestLeads
{
    private static int _leadId = 1000;

    public static Lead Create(
        int? id = null,
        int agencyId = 1,
        int brokerId = 1,
        int propertyId = 1,
        string fullName = "Lead Test",
        string? email = "lead.test@example.com",
        string? phoneNumber = null,
        string? message = "Please contact me",
        LeadStatus status = LeadStatus.New,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        var now = DateTime.UtcNow;
        var created = createdAt ?? now;
        var updated = updatedAt ?? created;

        return new Lead
        {
            Id = id ?? Interlocked.Increment(ref _leadId),
            AgencyId = agencyId,
            BrokerId = brokerId,
            PropertyId = propertyId,
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            Message = message,
            Status = status,
            CreatedAt = created,
            UpdatedAt = updated,
        };
    }
}