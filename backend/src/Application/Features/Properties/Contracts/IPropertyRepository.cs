using RealEstate.Application.Features.Properties.List;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Properties.Contracts;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(int id, CancellationToken ct);

    Task<Property?> GetByAgencyBrokerAndIdAsync(
        int agencyId,
        int brokerId,
        int propertyId,
        CancellationToken ct);

    Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken ct);

    Task CreateAsync(Property entity, CancellationToken ct);
    Task<bool> UpdateAsync(Property entity, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);

    Task<(IReadOnlyList<Property> Items, long TotalItems)> GetListAsync(
        PropertyListQuery query,
        CancellationToken ct);
}