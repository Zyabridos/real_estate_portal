namespace RealEstate.Application.Features.Agencies.Update;

public sealed record UpdateAgencyRequest(
    Guid Id,
    string Name,
    string PhoneNumber,
    string City, 
    string Street,
    string ZipCode            
);