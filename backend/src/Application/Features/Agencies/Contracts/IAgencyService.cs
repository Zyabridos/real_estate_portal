using RealEstate.Application.Common;
using RealEstate.Application.Features.Agency.Create;
using RealEstate.Application.Features.Agency.GetById;
using RealEstate.Application.Features.Agency.List;
using RealEstate.Application.Features.Agency.Update;

namespace RealEstate.Application.Features.Agency.Contracts;

public interface IAgencyService
{
    Task<PagedResult<AgencyListItemDto>> GetListAsync(AgencyListQuery query, CancellationToken ct);
    Task<AgencyDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<AgencyDetailsDto> CreateAsync(CreateAgencyRequest request, CancellationToken ct);
    Task<AgencyDetailsDto?> UpdateAsync(Guid id, UpdateAgencyRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}