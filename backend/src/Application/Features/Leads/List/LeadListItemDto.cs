using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Application.Features.Leads.List;

public sealed record LeadListItemDto(
    int Id,
    int AgencyId,
    int BrokerId,
    int PropertyId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    LeadStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);