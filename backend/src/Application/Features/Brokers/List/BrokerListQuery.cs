using RealEstate.Domain.Enums.Brokers;
using RealEstate.Domain.Enums.Common;

namespace RealEstate.Application.Features.Brokers.List;

public sealed record BrokerListQuery(
    int? BrokerId = null,
    string? FirstName = null,
    string? LastName = null,
    int? AgencyId = null,
    string? Email = null,
    string? PhoneNumber = null,
    int Page = 1,
    int PageSize = 20,
    SortBy? SortBy = null,
    SortDirection? SortDirection = null
);