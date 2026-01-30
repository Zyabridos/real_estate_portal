using AutoMapper;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Brokers.Create;
using RealEstate.Application.Features.Brokers.GetById;
using RealEstate.Application.Features.Brokers.List;
using RealEstate.Application.Features.Brokers.Update;
using RealEstate.Application.Features.Brokers.Contracts;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Features.Brokers.Services;

public sealed class BrokerService : IBrokerService
{
    private readonly IBrokerRepository _brokerRepository;
    private readonly IMapper _mapper;

    public BrokerService(IBrokerRepository brokerRepository, IMapper mapper)
    {
        _brokerRepository = brokerRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<BrokerListItemDto>> GetListAsync(BrokerListQuery query, CancellationToken ct)
    {
        var (items, totalItems) = await _brokerRepository.GetListAsync(query, ct);

        var dtoItems = _mapper.Map<IReadOnlyList<BrokerListItemDto>>(items);

        return new PagedResult<BrokerListItemDto>
        {
            Items = dtoItems,
            TotalItems = totalItems,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<BrokerDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _brokerRepository.GetById(id, ct);
        return entity is null ? null : _mapper.Map<BrokerDetailsDto>(entity);
    }

    public async Task<BrokerDetailsDto> CreateAsync(CreateBrokerRequest request, CancellationToken ct)
    {
        var entity = _mapper.Map<Broker>(request);

        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;

        entity.Email = entity.Email.Trim().ToLowerInvariant();
        entity.PhoneNumber = new string(entity.PhoneNumber
            .Trim()
            .Where(c => char.IsDigit(c) || c == '+')
            .ToArray());

        await _brokerRepository.CreateAsync(entity, ct);
        return _mapper.Map<BrokerDetailsDto>(entity);
    }


    public async Task<BrokerDetailsDto?> UpdateAsync(Guid id, UpdateBrokerRequest request, CancellationToken ct)
    {
        var entity = await _brokerRepository.GetById(id, ct);
        if (entity is null) return null;
        
        _mapper.Map(request, entity);

        entity.Email = entity.Email.Trim().ToLowerInvariant();
        entity.PhoneNumber = new string(entity.PhoneNumber.Trim().Where(c => char.IsDigit(c) || c == '+').ToArray());

        var updated = await _brokerRepository.UpdateAsync(entity, ct);

        return updated ? _mapper.Map<BrokerDetailsDto>(entity) : null;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) =>
        _brokerRepository.DeleteAsync(id, ct);
}
