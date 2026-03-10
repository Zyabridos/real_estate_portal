using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Domain.Entities;

// For simplicity, the model is basic, but
// TODO: eventually add (uncomment):
public sealed class Lead
{
    public int Id { get; set; }

    public int AgencyId { get; set; }
    public int BrokerId { get; set; }
    public int PropertyId { get; set; }
    public string FullName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
    public LeadStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // public Consents { get; set; } (List?)
    
}