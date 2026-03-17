using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Domain.Entities;

public sealed class Broker
{
    public int Id { get; set; }
    public int AgencyId { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? PhotoUrl { get; set; }
    public string? MainImageUrl { get; set; }

    public BrokerGender Gender { get; set; } = BrokerGender.Unspecified;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}