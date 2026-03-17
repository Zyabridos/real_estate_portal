using RealEstate.Application.Common;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;

namespace RealEstate.Application.Features.Properties.Contracts;

public interface IPropertyService
{
    Task<PagedResult<PropertyListItemDto>> GetListAsync(PropertyListQuery query, CancellationToken ct);
    Task<PropertyDetailsDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<PropertyDetailsDto> CreateAsync(CreatePropertyRequest request, CancellationToken ct);
    Task<PropertyDetailsDto?> UpdateAsync(int id, UpdatePropertyRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
