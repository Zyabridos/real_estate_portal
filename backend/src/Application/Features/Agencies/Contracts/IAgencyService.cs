using RealEstate.Application.Common;
using RealEstate.Application.Features.Agencies.Create;
using RealEstate.Application.Features.Agencies.GetById;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Application.Features.Agencies.Update;

namespace RealEstate.Application.Features.Agencies.Contracts;

public interface IAgencyService
{
    Task<PagedResult<AgencyListItemDto>> GetListAsync(AgencyListQuery query, CancellationToken ct);
    Task<AgencyDetailsDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<AgencyDetailsDto> CreateAsync(CreateAgencyRequest request, CancellationToken ct);
    Task<AgencyDetailsDto?> UpdateAsync(int id, UpdateAgencyRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}