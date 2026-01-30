using RealEstate.Application.Common;
using RealEstate.Application.Features.Brokers.Create;
using RealEstate.Application.Features.Brokers.GetById;
using RealEstate.Application.Features.Brokers.List;
using RealEstate.Application.Features.Brokers.Update;

namespace RealEstate.Application.Features.Brokers.Contracts;

public interface IBrokerService
{
    Task<PagedResult<BrokerListItemDto>> GetListAsync(BrokerListQuery query, CancellationToken ct);
    Task<BrokerDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<BrokerDetailsDto> CreateAsync(CreateBrokerRequest request, CancellationToken ct);
    Task<BrokerDetailsDto?> UpdateAsync(Guid id, UpdateBrokerRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}