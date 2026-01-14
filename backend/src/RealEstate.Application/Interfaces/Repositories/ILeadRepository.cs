using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Interfaces.Repositories;

public interface ILeadRepository
{
    Task<Lead?> FindByIdAsync(Guid id, CancellationToken ct);

    Task CreateAsync(Lead entity, CancellationToken ct);
    Task<bool> UpdateStatusAsync(Guid id, LeadStatus status, CancellationToken ct);

    Task<IReadOnlyList<Lead>> FindByPropertyIdAsync(Guid propertyId, int limit, CancellationToken ct);
}