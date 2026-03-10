using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.Features.Properties.Update;

public sealed record UpdatePropertyRequest(
    int BrokerId,
    string Title,
    string? Description,
    string Address,
    string City,
    decimal Price,
    int Bedrooms,
    int Bathrooms,
    decimal Area,
    string? MainImageUrl,
    PropertyType Type,
    PropertyStatus Status
);