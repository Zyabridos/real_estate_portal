using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Application.Features.Brokers.Create;

// Server assigns id and CreatedAt;
// AgencyId is fetched from DB (i. e. first check that entity - Agency - exists, then Broker.AgencyId = agencyId)
public sealed record CreateBrokerRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    BrokerGender Gender,
    string? PhotoUrl = null
);
