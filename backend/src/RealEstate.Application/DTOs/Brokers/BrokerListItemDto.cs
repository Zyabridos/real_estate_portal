namespace RealEstate.Application.DTOs.Brokers;

public sealed record BrokerListItemDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? PhotoUrl
);