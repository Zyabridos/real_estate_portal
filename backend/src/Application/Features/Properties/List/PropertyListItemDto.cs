using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.Features.Properties.List;

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
