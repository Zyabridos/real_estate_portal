namespace RealEstate.Application.DTOs.Leads;

public sealed record CreateLeadRequest(
    Guid PropertyId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? Message
);