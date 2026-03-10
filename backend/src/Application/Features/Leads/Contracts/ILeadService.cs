using RealEstate.Application.Common;
using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;

namespace RealEstate.Application.Features.Leads.Contracts;

public interface ILeadService
{
    Task<PagedResult<LeadListItemDto>> GetListAsync(LeadListQuery query, CancellationToken ct);
    Task<LeadDetailsDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<LeadDetailsDto> CreateAsync(CreateLeadRequest request, CancellationToken ct);
    Task<LeadDetailsDto?> UpdateAsync(int id, UpdateLeadRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
