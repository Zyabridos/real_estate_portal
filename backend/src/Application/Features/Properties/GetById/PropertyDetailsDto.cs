using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.Features.Properties.GetById;

public sealed record PropertyDetailsDto(
    int Id,
    int AgencyId,
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
    PropertyStatus Status,
    PropertyType Type,
    DateTime CreatedAt,
    DateTime UpdatedAt
);