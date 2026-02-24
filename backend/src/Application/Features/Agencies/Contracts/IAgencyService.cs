using RealEstate.Application.Common;
using RealEstate.Application.Features.Agencies.Create;
using RealEstate.Application.Features.Agencies.GetById;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Application.Features.Agencies.Update;

namespace RealEstate.Application.Features.Agencies.Contracts;

public interface IAgencyService
{
    Task<PagedResult<AgencyListItemDto>> GetListAsync(AgencyListQuery query, CancellationToken ct);
    Task<AgencyDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<AgencyDetailsDto> CreateAsync(CreateAgencyRequest request, CancellationToken ct);
    Task<AgencyDetailsDto?> UpdateAsync(Guid id, UpdateAgencyRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}