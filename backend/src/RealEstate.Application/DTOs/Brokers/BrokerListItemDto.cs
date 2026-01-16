using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Application.DTOs.Brokers;

public sealed record BrokerListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);