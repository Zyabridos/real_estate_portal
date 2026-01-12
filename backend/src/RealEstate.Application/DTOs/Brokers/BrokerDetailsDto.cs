namespace RealEstate.Application.DTOs.Brokers;

public sealed record BrokerDetailsDto(
    string Id,
    string AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? PhotoUrl
);