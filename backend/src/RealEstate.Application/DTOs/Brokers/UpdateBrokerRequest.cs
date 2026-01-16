namespace RealEstate.Application.DTOs.Brokers;

// It is server`s job to assign BrokerId and CreatedAt
public sealed record UpdateBrokerRequest(
    Guid AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl = null
);