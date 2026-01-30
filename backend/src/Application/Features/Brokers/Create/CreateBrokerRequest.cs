namespace RealEstate.Application.Features.Brokers.Create;

// It is server`s job to assign BrokerId and CreatedAt
public sealed record CreateBrokerRequest(
    Guid AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl = null
);