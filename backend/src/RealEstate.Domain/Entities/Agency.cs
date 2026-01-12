namespace RealEstate.Domain.Entities;

public sealed class Agency
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? OrgNumber { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}