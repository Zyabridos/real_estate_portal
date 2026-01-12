namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyDetailsDto(
    string Id,
    string Title,
    string Description,
    string Address,
    string City,
    decimal Price,
    string Type,
    int Bedrooms,
    int Bathrooms,
    decimal Area,
    string Status,
    string? MainImageUrl,
    string BrokerId,
    DateTime CreatedAt
);