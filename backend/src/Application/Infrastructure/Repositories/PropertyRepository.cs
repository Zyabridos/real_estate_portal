using MongoDB.Driver;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Queries.Properties;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Properties;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection;

    public PropertyRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Property>(MongoCollectionNames.Properties);
    }

    public Task<Property?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public Task CreateAsync(Property entity, CancellationToken ct) =>
        _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task<bool> UpdateAsync(Property entity, CancellationToken ct)
    {
        var res = await _collection.ReplaceOneAsync(
            x => x.Id == entity.Id,
            entity,
            cancellationToken: ct);

        return res.MatchedCount == 1;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var res = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return res.DeletedCount == 1;
    }

    public async Task<IReadOnlyList<Property>> FindByBrokerIdAsync(Guid brokerId, int limit, CancellationToken ct)
    {
        var items = await _collection.Find(x => x.BrokerId == brokerId)
            .SortByDescending(x => x.CreatedAt)
            .Limit(limit)
            .ToListAsync(ct);

        return items;
    }

    public async Task<(IReadOnlyList<Property> Items, long Total)> GetListAsync(
        PropertyListQuery query,
        CancellationToken ct)
    {
        var builder = Builders<Property>.Filter;
        var filters = new List<FilterDefinition<Property>>();

        if (!string.IsNullOrWhiteSpace(query.City))
            filters.Add(builder.Eq(x => x.City, query.City));

        if (!string.IsNullOrWhiteSpace(query.Type) &&
            Enum.TryParse<PropertyType>(query.Type, true, out var type))
            filters.Add(builder.Eq(x => x.Type, type));

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<PropertyStatus>(query.Status, true, out var status))
            filters.Add(builder.Eq(x => x.Status, status));

        if (query.MinPrice.HasValue)
            filters.Add(builder.Gte(x => x.Price, query.MinPrice.Value));

        if (query.MaxPrice.HasValue)
            filters.Add(builder.Lte(x => x.Price, query.MaxPrice.Value));
        
        if (query.BrokerId.HasValue)
        {
            filters.Add(builder.Eq(x => x.BrokerId, query.BrokerId.Value));
        }
        
        var filter = filters.Count == 0 ? builder.Empty : builder.And(filters);

        var sort = query.Sort?.ToLowerInvariant() switch
        {
            "priceasc" => Builders<Property>.Sort.Ascending(x => x.Price),
            "pricedesc" => Builders<Property>.Sort.Descending(x => x.Price),
            "createdatdesc" => Builders<Property>.Sort.Descending(x => x.CreatedAt),
            null or "" => Builders<Property>.Sort.Descending(x => x.CreatedAt),
            _ => Builders<Property>.Sort.Descending(x => x.CreatedAt)
        };

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection.Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
