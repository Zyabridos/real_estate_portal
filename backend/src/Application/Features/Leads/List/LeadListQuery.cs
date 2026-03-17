using RealEstate.Domain.Enums.Common;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Application.Features.Leads.List;

public sealed record LeadListQuery(
    int? Id = null,
    int? AgencyId = null,
    int? BrokerId = null,
    int? PropertyId = null,
    string? FullName = null,
    string? Email = null,
    string? PhoneNumber = null,
    int Page = 1,
    int PageSize = 20,
    LeadSortBy? SortBy = null,
    SortDirection? SortDirection = null
);