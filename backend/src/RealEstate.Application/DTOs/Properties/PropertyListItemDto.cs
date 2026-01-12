namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyListItemDto(
    string Id,
    string Title,
    string City,
    decimal Price,
    string Type,
    string Status,
    string? MainImageUrl
);