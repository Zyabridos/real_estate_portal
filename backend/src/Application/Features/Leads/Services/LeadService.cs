using AutoMapper;
using RealEstate.Application.Common;
using RealEstate.Application.Common.Abstractions;
using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;
using RealEstate.Application.Features.Leads.Contracts;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Leads;
using RealEstate.Application.Features.Leads.GetById;

namespace RealEstate.Application.Features.Leads.Services;

public sealed class LeadService : ILeadService
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;
    private readonly ISequenceGenerator _sequenceGenerator;

    public LeadService(
        ILeadRepository leadRepository,
        IMapper mapper,
        ISequenceGenerator sequenceGenerator)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
        _sequenceGenerator = sequenceGenerator;
    }

    public async Task<PagedResult<LeadListItemDto>> GetListAsync(LeadListQuery query, CancellationToken ct)
    {
        var (items, totalItems) = await _leadRepository.GetListAsync(query, ct);

        var dtoItems = _mapper.Map<IReadOnlyList<LeadListItemDto>>(items);

        return new PagedResult<LeadListItemDto>
        {
            Items = dtoItems,
            TotalItems = totalItems,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<LeadDetailsDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await _leadRepository.GetByIdAsync(id, ct);

        return entity is null
            ? null
            : _mapper.Map<LeadDetailsDto>(entity);
    }

    public async Task<LeadDetailsDto> CreateAsync(CreateLeadRequest request, CancellationToken ct)
    {
        var entity = _mapper.Map<Lead>(request);

        entity.Id = await _sequenceGenerator.GetNextValueAsync("leads", ct);
        entity.Status = LeadStatus.New;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = entity.CreatedAt;

        entity.Email = NormalizeEmail(entity.Email);
        entity.PhoneNumber = NormalizePhone(entity.PhoneNumber) ?? string.Empty;

        await _leadRepository.CreateAsync(entity, ct);

        return _mapper.Map<LeadDetailsDto>(entity);
    }

    public async Task<LeadDetailsDto?> UpdateAsync(int id, UpdateLeadRequest request, CancellationToken ct)
    {
        var entity = await _leadRepository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        _mapper.Map(request, entity);

        entity.Email = NormalizeEmail(entity.Email);
        entity.PhoneNumber = NormalizePhone(entity.PhoneNumber) ?? string.Empty;
        entity.UpdatedAt = DateTime.UtcNow;

        var updated = await _leadRepository.UpdateAsync(entity, ct);

        return updated ? _mapper.Map<LeadDetailsDto>(entity) : null;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct) =>
        _leadRepository.DeleteAsync(id, ct);

    private static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return email.Trim().ToLowerInvariant();
    }

    private static string? NormalizePhone(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

        var cleaned = new string(
            phoneNumber
                .Trim()
                .Where(c => char.IsDigit(c) || c == '+')
                .ToArray()
        );

        return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
    }
}