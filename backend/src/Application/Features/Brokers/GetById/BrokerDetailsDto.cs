namespace RealEstate.Application.Features.Brokers.GetById;

public sealed record BrokerDetailsDto(
    Guid Id,
    Guid AgencyId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? PhotoUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);