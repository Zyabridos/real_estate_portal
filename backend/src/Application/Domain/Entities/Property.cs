using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Domain.Entities;

public sealed class Property
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public int BrokerId { get; set; }

    public string Title { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal Area { get; set; }
    public string? MainImageUrl { get; set; }

    public PropertyStatus Status { get; set; }
    public PropertyType Type { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}