using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public sealed class Lead
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }
    public LeadStatus Status { get; set; }
}