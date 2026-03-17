namespace RealEstate.Application.Features.Agencies.Update;

public sealed record UpdateAgencyRequest(
    int Id,
    string Name,
    string PhoneNumber,
    string City, 
    string Street,
    string ZipCode            
);