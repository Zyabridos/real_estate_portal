using AutoMapper;
using RealEstate.Application.Common;
using RealEstate.Application.Common.Abstractions;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Features.Properties.GetById;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Application.Features.Properties.Update;
using RealEstate.Application.Features.Properties.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Properties.Services;

public sealed class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;
    private readonly ISequenceGenerator _sequenceGenerator;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IMapper mapper,
        ISequenceGenerator sequenceGenerator)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
        _sequenceGenerator = sequenceGenerator;
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

    public async Task<PropertyDetailsDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _propertyRepository.GetByIdAsync(id, ct);

        return entity is null
            ? null
            : _mapper.Map<PropertyDetailsDto>(entity);
    }

    public async Task<PropertyDetailsDto> CreateAsync(CreatePropertyRequest request, CancellationToken ct)
    {
        var entity = _mapper.Map<Property>(request);

        entity.Id = await _sequenceGenerator.GetNextValueAsync("properties", ct);
        entity.CreatedAt = DateTime.UtcNow;

        await _propertyRepository.CreateAsync(entity, ct);

        return _mapper.Map<PropertyDetailsDto>(entity);
    }

    public async Task<PropertyDetailsDto?> UpdateAsync(int id, UpdatePropertyRequest request, CancellationToken ct)
    {
        var entity = await _propertyRepository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        _mapper.Map(request, entity);

        var updated = await _propertyRepository.UpdateAsync(entity, ct);
        return updated ? _mapper.Map<PropertyDetailsDto>(entity) : null;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct) =>
        _propertyRepository.DeleteAsync(id, ct);
}