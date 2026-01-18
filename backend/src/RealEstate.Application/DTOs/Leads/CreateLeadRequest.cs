namespace RealEstate.Application.DTOs.Leads;

public sealed record CreateLeadRequest(
    string PropertyId,
    string Name,
    string PhoneNumber,
    string? Email,
    string? Message
);
