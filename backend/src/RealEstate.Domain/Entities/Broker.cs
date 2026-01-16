namespace RealEstate.Domain.Entities;

public sealed class Broker
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? PhotoUrl { get; set; }
    public string? MainImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}