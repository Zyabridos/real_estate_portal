namespace RealEstate.Application.DTOs.Brokers;

public sealed record CreateBrokerRequest(
    Guid BrokerId,
    Guid AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl = null
);