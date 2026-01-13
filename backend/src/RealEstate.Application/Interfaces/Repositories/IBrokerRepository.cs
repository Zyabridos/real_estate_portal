using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories;

public interface IBrokerRepository
{
    Task<Broker?> FindByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Broker>> GetAllAsync(CancellationToken ct);

    Task CreateAsync(Broker entity, CancellationToken ct);
    Task<bool> UpdateAsync(Broker entity, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}