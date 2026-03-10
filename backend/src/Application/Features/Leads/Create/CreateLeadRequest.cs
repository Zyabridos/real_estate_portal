namespace RealEstate.Application.Features.Leads.Create;

public sealed record CreateLeadRequest(
    int PropertyId, // Need it in Create request => find property by id, fetch BrokerId and AgencyId from DB, assign last two to the lead 
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? Message
);