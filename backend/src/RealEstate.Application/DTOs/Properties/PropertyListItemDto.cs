using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyListItemDto(
    Guid Id,
    string Title,
    string City,
    decimal Price,
    PropertyType Type,
    PropertyStatus Status,
    string? MainImageUrl,
    DateTime CreatedAt
);
