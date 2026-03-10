using RealEstate.Application.Features.Agencies.List;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Agencies.Contracts;

public interface IAgencyRepository
{
    Task<Agency?> GetById(int id, CancellationToken ct);
    Task<IReadOnlyList<Agency>> GetAllAsync(CancellationToken ct);

    Task CreateAsync(Agency entity, CancellationToken ct);
    Task<bool> UpdateAsync(Agency entity, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    
    Task<(IReadOnlyList<Agency> Items, long TotalItems)> GetListAsync(
        AgencyListQuery query,
        CancellationToken ct);
}