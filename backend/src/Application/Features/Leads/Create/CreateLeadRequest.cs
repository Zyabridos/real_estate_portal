namespace RealEstate.Application.Features.Leads.Create;

public sealed record CreateLeadRequest(
    Guid PropertyId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? Message
);