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
    string? MainImageUrl,
    IReadOnlyList<string> ImageUrls,
    PropertyType Type,
    PropertyStatus Status
);