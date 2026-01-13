using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Queries.Properties;

namespace RealEstate.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PagedResult<PropertyListItemDto>> GetListAsync(
        PropertyListQuery query,
        CancellationToken cancellationToken
    );

    Task<PropertyDetailsDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken
    );
}