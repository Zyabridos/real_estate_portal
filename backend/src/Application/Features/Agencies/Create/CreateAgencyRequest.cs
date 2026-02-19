namespace RealEstate.Application.Features.Agencies.Create;

public sealed record CreateAgencyRequest(
    string Name,
    string OrgNumber,
    string PhoneNumber,
    string City,
    string Street,
    string ZipCode,
);