namespace RealEstate.Application.DTOs.Leads;

public sealed record UpdateLeadRequest(
    string? FullName,
    string? Email,
    string? PhoneNumber,
    string? Message
);