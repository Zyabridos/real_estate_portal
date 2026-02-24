namespace RealEstate.Domain.Entities;

public sealed class Agency
{
    public Guid Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string OrgNumber { get; set; }  = default!;
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime  UpdatedAt { get; set; }
}