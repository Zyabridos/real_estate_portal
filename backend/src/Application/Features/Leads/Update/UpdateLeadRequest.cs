namespace RealEstate.Application.Features.Leads.Update;

public sealed record UpdateLeadRequest(
    string? FullName,
    string? Email,
    string? PhoneNumber,
    string? Message
);