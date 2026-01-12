namespace RealEstate.Application.DTOs.Leads;

public sealed record CreateLeadRequest(
    string PropertyId,
    string Name,
    string Email,
    string? Phone,
    string? Message
);