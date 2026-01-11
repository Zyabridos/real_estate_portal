using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public interface IPropertyRepository
{
    Task<Property?> FindByIdAsync(Guid id, CancellationToken ct);

    Task CreateAsync(Property entity, CancellationToken ct);
    Task<bool> UpdateAsync(Property entity, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<PagedResult<Property>> FindPagedAsync(
        string? city,
        PropertyType? type,
        PropertyStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        CancellationToken ct);

    Task<IReadOnlyList<Property>> FindByBrokerIdAsync(Guid brokerId, int limit, CancellationToken ct);
}