using RealEstate.Domain.Enums.Properties;

namespace RealEstate.Application.Features.Properties.List;

public sealed record PropertyListQuery(
    string? City = null,
    PropertyType? Type = null,
    PropertyStatus? Status = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int? AgencyId = null,
    int? BrokerId = null,
    int Page = 1,
    int PageSize = 20,
    string? Sort = null
);