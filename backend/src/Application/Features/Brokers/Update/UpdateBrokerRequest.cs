using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Application.Features.Brokers.Update;

// It is server`s job to assign CreatedAt and UpdatedAt
public sealed record UpdateBrokerRequest(
    int AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    BrokerGender Gender,
    string? PhotoUrl = null
);