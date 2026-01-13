using AutoMapper;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Application.Queries.Properties;
using RealEstate.Domain.Enums;
using RealEstate.Domain.Entities;

public sealed class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<PagedResult<PropertyListItemDto>> GetListAsync(PropertyListQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await _propertyRepository.GetListAsync(query, ct);

        var dtoItems = items.Select(x => new PropertyListItemDto(
            x.Id, x.Title, x.City, x.Price, x.Type, x.Status, x.MainImageUrl
        )).ToList();

        return new PagedResult<PropertyListItemDto>
        {
            Items = dtoItems,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PropertyDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _propertyRepository.GetByIdAsync(id, ct);
        return entity is null ? null : ToDetailsDto(entity);
    }

    public async Task<PropertyDetailsDto> CreateAsync(CreatePropertyRequest request, CancellationToken ct)
    {
        var entity = new Property
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            Price = request.Price,
            Type = request.Type,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Area = request.Area,
            Status = request.Status,
            MainImageUrl = request.MainImageUrl,
            BrokerId = request.BrokerId,
            CreatedAt = DateTime.UtcNow
        };

        await _propertyRepository.CreateAsync(entity, ct);
        return ToDetailsDto(entity);
    }

    public async Task<PropertyDetailsDto?> UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken ct)
    {
        var entity = await _propertyRepository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Address = request.Address;
        entity.City = request.City;
        entity.Price = request.Price;
        entity.Type = request.Type;
        entity.Bedrooms = request.Bedrooms;
        entity.Bathrooms = request.Bathrooms;
        entity.Area = request.Area;
        entity.Status = request.Status;
        entity.MainImageUrl = request.MainImageUrl;
        entity.BrokerId = request.BrokerId;

        var updated = await _propertyRepository.UpdateAsync(entity, ct);
        return updated ? ToDetailsDto(entity) : null;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _propertyRepository.DeleteAsync(id, ct);

    private static PropertyDetailsDto ToDetailsDto(Property x) => new(
        x.Id, x.Title, x.Description, x.Address, x.City, x.Price,
        x.Type, x.Bedrooms, x.Bathrooms, x.Area, x.Status,
        x.MainImageUrl, x.BrokerId, x.CreatedAt
    );
}
