using RealEstate.Domain.Enums.Agencies;

namespace RealEstate.Application.Features.Agencies.List;

public sealed record AgencyListItemDto(
    int Id,
    string Name,
    string OrgNumber,
    string? PhoneNumber,
    string? City,
    string? Street,
    string? ZipCode,
    DateTime CreatedAt,
    DateTime UpdatedAt
);