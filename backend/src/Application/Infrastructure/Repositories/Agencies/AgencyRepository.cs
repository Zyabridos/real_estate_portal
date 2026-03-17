using MongoDB.Driver;
using System.Linq.Expressions;
using RealEstate.Application.Features.Agencies.Contracts;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Agencies;
using DomainSortDirection = RealEstate.Domain.Enums.Common.SortDirection;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories.Agencies;

public sealed class AgencyRepository : IAgencyRepository
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IMongoCollection<Agency> _collection;

    public AgencyRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Agency>(MongoCollectionNames.Agencies);
    }
    public Task<Agency?> GetById(int id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Agency>> GetAllAsync(CancellationToken ct) =>
        await _collection.Find(Builders<Agency>.Filter.Empty)
            .SortBy(x => x.Name)
            .ToListAsync(ct);

    public Task CreateAsync(Agency entity, CancellationToken ct)
    {
        NormalizeForPersistence(entity);
        return _collection.InsertOneAsync(entity, cancellationToken: ct);
    }

    public async Task<bool> UpdateAsync(Agency entity, CancellationToken ct)
    {
        NormalizeForPersistence(entity);

        var res = await _collection.ReplaceOneAsync(
            x => x.Id == entity.Id,
            entity,
            cancellationToken: ct);
        
        return res.IsAcknowledged && res.MatchedCount == 1;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var res = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return res.IsAcknowledged && res.DeletedCount == 1;
    }

    public async Task<(IReadOnlyList<Agency> Items, long TotalItems)> GetListAsync(
        AgencyListQuery query,
        CancellationToken ct)
    {
        var builder = Builders<Agency>.Filter;
        var filters = new List<FilterDefinition<Agency>>();

        // Equality filters
        if (query.Id.HasValue)
            filters.Add(builder.Eq(x => x.Id, query.Id.Value));
        
        if (!string.IsNullOrWhiteSpace(query.Name))
            filters.Add(builder.Eq(x => x.Name, query.Name.Trim()));
        
        if (!string.IsNullOrWhiteSpace(query.OrgNumber))
            filters.Add(builder.Eq(x => x.OrgNumber, query.OrgNumber.Trim()));

        if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
        {
            var phone = NormalizePhone(query.PhoneNumber);
            filters.Add(builder.Eq(x => x.PhoneNumber, phone));
        }

        var filter = filters.Count == 0 ? builder.Empty : builder.And(filters);

        // Sorting
        var sortBy = query.SortBy ?? SortBy.CreatedAt;
        var direction = query.SortDirection ?? DomainSortDirection.Desc;
        var sort = BuildSort(sortBy, direction);

        // Paging
        var page = query.Page < 1 ? 1 : query.Page;

        var pageSize = query.PageSize < 1 ? DefaultPageSize : query.PageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        var skip = (page - 1) * pageSize;

        var totalItems = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection.Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (items, totalItems);
    }

    private static SortDefinition<Agency> BuildSort(SortBy sortBy, DomainSortDirection direction)
    {
        var sortBuilder = Builders<Agency>.Sort;

        SortDefinition<Agency> Apply(Expression<Func<Agency, object>> field) =>
            direction == DomainSortDirection.Asc
                ? sortBuilder.Ascending(field)
                : sortBuilder.Descending(field);

        return sortBy switch
        {
            SortBy.Name         => Apply(x => x.Name),
            SortBy.OrgNumber    => Apply(x => x.OrgNumber),
            SortBy.City         => Apply(x => x.City),
            SortBy.CreatedAt    => Apply(x => x.CreatedAt),
            _                   => Apply(x => x.CreatedAt)
        };
    }

    private static void NormalizeForPersistence(Agency entity)
    {
        entity.PhoneNumber = NormalizePhone(entity.PhoneNumber);
    }

    private static string NormalizePhone(string phone)
    {
        phone = phone.Trim();

        var chars = phone
            .Where(c => char.IsDigit(c) || c == '+')
            .ToArray();

        return new string(chars);
    }
}
