using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Leads;

public sealed record LeadListItemDto(
    Guid Id,
    Guid PropertyId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    LeadStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);