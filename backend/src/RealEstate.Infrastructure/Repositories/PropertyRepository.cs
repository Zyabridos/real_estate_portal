using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection;

    public PropertyRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Property>(MongoCollectionNames.Properties);
    }

    public Task<Property?> FindByIdAsync(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public Task CreateAsync(Property entity, CancellationToken ct) =>
        _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task<bool> UpdateAsync(Property entity, CancellationToken ct)
    {
        var res = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: ct);
        return res.MatchedCount == 1 && res.ModifiedCount == 1;
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

    public async Task<PagedResult<Property>> FindPagedAsync(
        string? city,
        PropertyType? type,
        PropertyStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var filter = Builders<Property>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(city))
            filter &= Builders<Property>.Filter.Eq(x => x.City, city);

        if (type is not null)
            filter &= Builders<Property>.Filter.Eq(x => x.Type, type.Value);

        if (status is not null)
            filter &= Builders<Property>.Filter.Eq(x => x.Status, status.Value);

        if (minPrice is not null)
            filter &= Builders<Property>.Filter.Gte(x => x.Price, minPrice.Value);

        if (maxPrice is not null)
            filter &= Builders<Property>.Filter.Lte(x => x.Price, maxPrice.Value);

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Property>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

    }
}
