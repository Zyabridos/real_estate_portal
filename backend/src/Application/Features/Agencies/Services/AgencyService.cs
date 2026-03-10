using AutoMapper;
using RealEstate.Application.Common;
using RealEstate.Application.Common.Abstractions;
using RealEstate.Application.Features.Agencies.Create;
using RealEstate.Application.Features.Agencies.GetById;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Application.Features.Agencies.Update;
using RealEstate.Application.Features.Agencies.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Agencies.Services;

public sealed class AgencyService : IAgencyService
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IMapper _mapper;
    private readonly ISequenceGenerator _sequenceGenerator;

    public AgencyService(
        IAgencyRepository agencyRepository,
        IMapper mapper,
        ISequenceGenerator sequenceGenerator)
    {
        _agencyRepository = agencyRepository;
        _mapper = mapper;
        _sequenceGenerator = sequenceGenerator;
    }

    public async Task<PagedResult<AgencyListItemDto>> GetListAsync(AgencyListQuery query, CancellationToken ct)
    {
        var (items, totalItems) = await _agencyRepository.GetListAsync(query, ct);

        var dtoItems = _mapper.Map<IReadOnlyList<AgencyListItemDto>>(items);

        return new PagedResult<AgencyListItemDto>
        {
            Items = dtoItems,
            TotalItems = totalItems,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<AgencyDetailsDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _agencyRepository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<AgencyDetailsDto>(entity);
    }

    public async Task<AgencyDetailsDto> CreateAsync(CreateAgencyRequest request, CancellationToken ct)
    {
        var entity = _mapper.Map<Agency>(request);

        entity.Id = await _sequenceGenerator.GetNextValueAsync("agencies", ct);
        entity.CreatedAt = DateTime.UtcNow;
        
        entity.PhoneNumber = new string(entity.PhoneNumber
            .Trim()
            .Where(c => char.IsDigit(c) || c == '+')
            .ToArray());

        await _agencyRepository.CreateAsync(entity, ct);
        return _mapper.Map<AgencyDetailsDto>(entity);
    }


    public async Task<AgencyDetailsDto?> UpdateAsync(int id, UpdateAgencyRequest request, CancellationToken ct)
    {
        var entity = await _agencyRepository.GetById(id, ct);
        if (entity is null) return null;
        
        _mapper.Map(request, entity);
        
        entity.PhoneNumber = new string(entity.PhoneNumber.Trim().Where(c => char.IsDigit(c) || c == '+').ToArray());

        var updated = await _agencyRepository.UpdateAsync(entity, ct);

        return updated ? _mapper.Map<AgencyDetailsDto>(entity) : null;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct) =>
        _agencyRepository.DeleteAsync(id, ct);
}
