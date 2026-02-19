using RealEstate.Domain.Enums.Agencies;
using RealEstate.Domain.Enums.Common;

namespace RealEstate.Application.Features.Agencies.List;

public sealed record AgencyListQuery(
    Guid? Id = null,
    string? Name = null,
    string? OrgNumver = null,
    string? PhoneNumber = null,
    string? City = null,
    string? Street = null,
    string? ZipCode = null,
    int Page = 1,
    int PageSize = 20,
    SortBy? SortBy = null,
    SortDirection? SortDirection = null
);