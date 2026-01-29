using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Application.DTOs.Leads;

public sealed record LeadDetailsDto(
    Guid Id,
    Guid PropertyId,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string? Message,
    LeadStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);