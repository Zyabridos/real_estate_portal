using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Leads;
using RealEstate.Application.Queries.Leads;

namespace RealEstate.Application.Interfaces.Services;

public interface ILeadService
{
    Task<PagedResult<LeadListItemDto>> GetListAsync(LeadListQuery query, CancellationToken ct);
    Task<LeadDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<LeadDetailsDto> CreateAsync(CreateLeadRequest request, CancellationToken ct);
    Task<LeadDetailsDto?> UpdateAsync(Guid id, UpdateLeadRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
