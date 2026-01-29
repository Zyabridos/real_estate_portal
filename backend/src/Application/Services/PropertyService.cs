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
    private readonly IMapper _mapper;

    public PropertyService(IPropertyRepository propertyRepository, IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<PropertyListItemDto>> GetListAsync(PropertyListQuery query, CancellationToken ct)
    {
        var (items, totalItems) = await _propertyRepository.GetListAsync(query, ct);

        var dtoItems = _mapper.Map<IReadOnlyList<PropertyListItemDto>>(items);

        return new PagedResult<PropertyListItemDto>
        {
            Items = dtoItems,
            TotalItems = totalItems,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PropertyDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _propertyRepository.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<PropertyDetailsDto>(entity);
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
        return _mapper.Map<PropertyDetailsDto>(entity);
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
        return updated ? _mapper.Map<PropertyDetailsDto>(entity) : null;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _propertyRepository.DeleteAsync(id, ct);
}
