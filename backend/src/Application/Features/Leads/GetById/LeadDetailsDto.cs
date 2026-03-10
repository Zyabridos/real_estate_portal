using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Application.Features.Leads.GetById;

public sealed record LeadDetailsDto(
    int Id,
    int AgencyId,
    int BrokerId,
    int PropertyId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? Message,
    LeadStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);