using RealEstate.Application.Queries.Properties;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<(IReadOnlyList<Property> Items, long Total)> GetListAsync(
        PropertyListQuery query,
        CancellationToken ct);

    Task CreateAsync(Property entity, CancellationToken ct);
    Task<bool> UpdateAsync(Property entity, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Property>> FindByBrokerIdAsync(Guid brokerId, int limit, CancellationToken ct);
}