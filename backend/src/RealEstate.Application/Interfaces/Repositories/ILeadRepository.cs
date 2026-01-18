using RealEstate.Application.Queries.Leads;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories;

public interface ILeadRepository
{
    Task<(IReadOnlyList<Lead> Items, long TotalCount)> GetListAsync(LeadListQuery query, CancellationToken ct);

    Task<Lead?> GetByIdAsync(Guid id, CancellationToken ct);

    Task CreateAsync(Lead lead, CancellationToken ct);

    Task<bool> UpdateAsync(Lead lead, CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
