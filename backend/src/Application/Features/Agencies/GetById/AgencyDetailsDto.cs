namespace RealEstate.Application.Features.Agencies.GetById;

public sealed record AgencyDetailsDto(
    Guid Id,
    string Name,
    string OrgNumber,
    string PhoneNumber,
    string City,
    string Street,
    string ZipCode,
    DateTime CreatedAt,
    DateTime UpdatedAt
);