using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Application.Features.Brokers.List;

public sealed record BrokerListItemDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl,
    BrokerGender Gender,
    DateTime CreatedAt,
    DateTime UpdatedAt
);