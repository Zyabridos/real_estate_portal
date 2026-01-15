using RealEstate.Application.Queries.Brokers;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories;

public interface IBrokerRepository
{
    Task<Broker?> GetById(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Broker>> GetAllAsync(CancellationToken ct);

    Task CreateAsync(Broker entity, CancellationToken ct);
    Task<bool> UpdateAsync(Broker entity, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    
    // Returns brokers matching the query filters: current page as Items and total matches as TotalCount
    Task<(IReadOnlyList<Broker> Items, long TotalCount)> GetListAsync(
        BrokerListQuery query,
        CancellationToken ct);
}
