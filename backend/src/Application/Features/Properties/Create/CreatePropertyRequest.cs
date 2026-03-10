using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.Features.Properties.Create;

public sealed record CreatePropertyRequest(
    int BrokerId,
    string Title,
    string? Description,
    string Address,
    string City,
    decimal Price,
    decimal Area,
    int Bedrooms,
    int Bathrooms,
    PropertyType Type,
    PropertyStatus Status,
    string? MainImageUrl
);