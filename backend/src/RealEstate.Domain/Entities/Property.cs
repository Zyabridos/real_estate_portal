using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public sealed class Property
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;

    public decimal Price { get; set; }
    public PropertyType Type { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double Area { get; set; }

    public Guid BrokerId { get; set; }
    public PropertyStatus Status { get; set; }

    public string? MainImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}