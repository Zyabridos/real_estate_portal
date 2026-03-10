using RealEstate.Application.Features.Brokers.List;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Brokers.Contracts;

public interface IBrokerRepository
{
    Task<Broker?> GetById(int id, CancellationToken ct);
    Task<IReadOnlyList<Broker>> GetAllAsync(CancellationToken ct);

    Task CreateAsync(Broker entity, CancellationToken ct);
    Task<bool> UpdateAsync(Broker entity, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    
    Task<(IReadOnlyList<Broker> Items, long TotalItems)> GetListAsync(
        BrokerListQuery query,
        CancellationToken ct);
}
