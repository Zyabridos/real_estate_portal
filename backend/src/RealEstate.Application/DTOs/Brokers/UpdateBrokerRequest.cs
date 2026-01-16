namespace RealEstate.Application.DTOs.Brokers;

public sealed record UpdateBrokerRequest(
    Guid BrokerId,
    Guid AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl = null
);