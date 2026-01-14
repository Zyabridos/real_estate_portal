using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Queries.Properties;

namespace RealEstate.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PagedResult<PropertyListItemDto>> GetListAsync(PropertyListQuery query, CancellationToken ct);
    Task<PropertyDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<PropertyDetailsDto> CreateAsync(CreatePropertyRequest request, CancellationToken ct);
    Task<PropertyDetailsDto?> UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
