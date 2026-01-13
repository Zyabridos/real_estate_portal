using AutoMapper;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Application.Queries.Properties;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public PropertyService(IPropertyRepository propertyRepository, IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<PropertyListItemDto>> GetListAsync(
        PropertyListQuery query,
        CancellationToken cancellationToken)
    {
        var type = ParseNullableEnum<PropertyType>(query.Type);
        var status = ParseNullableEnum<PropertyStatus>(query.Status);

        var page = await _propertyRepository.FindPagedAsync(
            query.City,
            type,
            status,
            query.MinPrice,
            query.MaxPrice,
            query.Page,
            query.PageSize,
            cancellationToken
        );

        var items = _mapper.Map<List<PropertyListItemDto>>(page.Items);

        return new PagedResult<PropertyListItemDto>
        {
            Items = items,
            Page = page.Page,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount
        };
    }

    public async Task<PropertyDetailsDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return null;
        }

        var property = await _propertyRepository.FindByIdAsync(guid, cancellationToken);

        return property is null ? null : _mapper.Map<PropertyDetailsDto>(property);
    }

    private static TEnum? ParseNullableEnum<TEnum>(string? value)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Allow "active"/"Active", "sold"/"Sold", etc.
        return Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed)
            ? parsed
            : null;
    }
}
