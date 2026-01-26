using MongoDB.Driver;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Queries.Leads;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Common;
using DomainSortDirection = RealEstate.Domain.Enums.Common.SortDirection;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Infrastructure.Repositories;

public sealed class LeadRepository : ILeadRepository
{
    private const string CollectionName = "leads";
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;
    private readonly IMongoCollection<Lead> _collection;

    public LeadRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Lead>(CollectionName);
    }

    public Task CreateAsync(Lead lead, CancellationToken ct) =>
        _collection.InsertOneAsync(lead, cancellationToken: ct);

    public Task<Lead?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount == 1;
    }

    public async Task<bool> UpdateAsync(Lead lead, CancellationToken ct)
    {
        var result = await _collection.ReplaceOneAsync(
            filter: x => x.Id == lead.Id,
            replacement: lead,
            cancellationToken: ct);

        return result.ModifiedCount == 1;
    }

    public async Task<(IReadOnlyList<Lead> Items, long TotalItems)> GetListAsync(LeadListQuery query, CancellationToken ct)
    {
        var filter = BuildFilter(query);

        var totalItems = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var find = _collection.Find(filter);
        find = ApplySorting(find, query);
        find = ApplyPaging(find, query);

        var items = await find.ToListAsync(ct);

        return (items, totalItems);
    }

    private static FilterDefinition<Lead> BuildFilter(LeadListQuery query)
    {
        var builder = Builders<Lead>.Filter;
        var filter = builder.Empty;

        if (query.Id is not null)
        {
            filter &= builder.Eq(x => x.Id, query.Id.Value);
        }

        if (query.PropertyId is not null)
        {
            filter &= builder.Eq(x => x.PropertyId, query.PropertyId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(query.FullName))
        {
            filter &= builder.Eq(x => x.FullName, query.FullName.Trim().ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            filter &= builder.Eq(x => x.Email, query.Email.Trim().ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
        {
            var normalized = new string(query.PhoneNumber
                .Trim()
                .Where(c => char.IsDigit(c) || c == '+')
                .ToArray());

            filter &= builder.Eq(x => x.PhoneNumber, normalized);
        }

        return filter;
    }

    private static IFindFluent<Lead, Lead> ApplySorting(IFindFluent<Lead, Lead> find, LeadListQuery query)
    {
        var sortBy = query.SortBy ?? LeadSortBy.CreatedAt;
        var direction = query.SortDirection ?? DomainSortDirection.Desc;

        SortDefinition<Lead> primary = sortBy switch
        {
            LeadSortBy.CreatedAt   => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.CreatedAt)
                : Builders<Lead>.Sort.Descending(x => x.CreatedAt),

            LeadSortBy.UpdatedAt   => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.UpdatedAt)
                : Builders<Lead>.Sort.Descending(x => x.UpdatedAt),

            LeadSortBy.Status      => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.Status)
                : Builders<Lead>.Sort.Descending(x => x.Status),

            LeadSortBy.FullName    => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.FullName)
                : Builders<Lead>.Sort.Descending(x => x.FullName),

            LeadSortBy.Email       => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.Email)
                : Builders<Lead>.Sort.Descending(x => x.Email),

            LeadSortBy.PhoneNumber => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.PhoneNumber)
                : Builders<Lead>.Sort.Descending(x => x.PhoneNumber),

            LeadSortBy.PropertyId  => direction == DomainSortDirection.Asc
                ? Builders<Lead>.Sort.Ascending(x => x.PropertyId)
                : Builders<Lead>.Sort.Descending(x => x.PropertyId),

            _ => Builders<Lead>.Sort.Descending(x => x.CreatedAt)
        };

        // tie-breaker for stable sort
        var stable = primary
            .Ascending(x => x.CreatedAt)
            .Ascending(x => x.Id);

        return find.Sort(stable);
    }

    private static IFindFluent<Lead, Lead> ApplyPaging(IFindFluent<Lead, Lead> find, LeadListQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;

        var pageSize = query.PageSize < 1 ? DefaultPageSize : query.PageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        var skip = (page - 1) * pageSize;

        return find.Skip(skip).Limit(pageSize);
    }
}
