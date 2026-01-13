namespace RealEstate.Application.Queries.Properties;

public sealed record PropertyListQuery(
    string? City,
    string? Type,
    string? Status,
    decimal? MinPrice,
    decimal? MaxPrice,
    int Page = 1,
    int PageSize = 20,
    string? Sort = null
);