using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

// For simplicity, the model is basic, but
// TODO: eventually add (uncomment):
public sealed class Lead
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }
    // public Guid AssignedBrokerId { get; set; }
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
    public LeadStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // public Consents { get; set; } (List?)
    
}