using RealEstate.Application.Features.Leads.List;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Leads.Contracts;

public interface ILeadRepository
{
    Task<(IReadOnlyList<Lead> Items, long TotalItems)> GetListAsync(LeadListQuery query, CancellationToken ct);

    Task<Lead?> GetByIdAsync(Guid id, CancellationToken ct);

    Task CreateAsync(Lead lead, CancellationToken ct);

    Task<bool> UpdateAsync(Lead lead, CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
