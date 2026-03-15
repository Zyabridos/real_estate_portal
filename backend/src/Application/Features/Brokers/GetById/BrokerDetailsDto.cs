using RealEstate.Domain.Enums.Brokers;
    
namespace RealEstate.Application.Features.Brokers.GetById;

public sealed record BrokerDetailsDto(
    int Id,
    int AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl,
    BrokerGender Gender,
    DateTime CreatedAt,
    DateTime UpdatedAt
);