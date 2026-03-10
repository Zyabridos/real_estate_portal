using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.Features.Properties.List;

public sealed record PropertyListItemDto(
    int Id,
    int? AgencyId,
    int? BrokerId,
    string Title,
    string City,
    decimal Price,
    string? MainImageUrl,
    PropertyType Type,
    PropertyStatus Status,
    DateTime CreatedAt
);
