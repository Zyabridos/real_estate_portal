using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyDetailsDto(
    Guid Id,
    string Title,
    string? Description,
    string Address,
    string City,
    decimal Price,
    PropertyType Type,
    int Bedrooms,
    int Bathrooms,
    decimal Area,
    PropertyStatus Status,
    string? MainImageUrl,
    Guid BrokerId,
    DateTime CreatedAt
    // TODO: add UpdatedAt
);
