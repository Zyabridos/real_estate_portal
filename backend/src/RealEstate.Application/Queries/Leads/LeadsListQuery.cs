using RealEstate.Domain.Enums.Common;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Application.Queries.Leads;

public sealed record LeadListQuery(
    Guid? Id = null,
    Guid? PropertyId = null,
    string? FullName = null,
    string? Email = null,
    string? PhoneNumber = null,
    int Page = 1,
    int PageSize = 20,
    LeadSortBy? SortBy = null,
    SortDirection? SortDirection = null
);