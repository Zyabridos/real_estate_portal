using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Brokers;
using RealEstate.Application.Queries.Brokers;

namespace RealEstate.Application.Interfaces.Services;

public interface IBrokerService
{
    Task<PagedResult<BrokerListItemDto>> GetListAsync(BrokerListQuery query, CancellationToken ct);
    Task<BrokerDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<BrokerDetailsDto> CreateAsync(CreateBrokerRequest request, CancellationToken ct);
    Task<BrokerDetailsDto?> UpdateAsync(Guid id, UpdateBrokerRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}