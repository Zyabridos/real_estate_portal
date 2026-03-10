using MongoDB.Driver;
using RealEstate.Application.Features.Properties.Contracts;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories.Properties;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection;

    public PropertyRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Property>(MongoCollectionNames.Properties);
    }

    public Task<Property?> GetByIdAsync(int id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public Task<Property?> GetByAgencyBrokerAndIdAsync(
        int agencyId,
        int brokerId,
        int propertyId,
        CancellationToken ct) =>
        _collection.Find(x =>
            x.AgencyId == agencyId &&
            x.BrokerId == brokerId &&
            x.Id == propertyId)
        .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken ct) =>
        await _collection.Find(Builders<Property>.Filter.Empty).ToListAsync(ct);

    public Task CreateAsync(Property entity, CancellationToken ct) =>
        _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task<bool> UpdateAsync(Property entity, CancellationToken ct)
    {
        var result = await _collection.ReplaceOneAsync(
            x => x.Id == entity.Id,
            entity,
            cancellationToken: ct);

        return result.IsAcknowledged && result.MatchedCount == 1;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.IsAcknowledged && result.DeletedCount == 1;
    }

    public async Task<(IReadOnlyList<Property> Items, long TotalItems)> GetListAsync(
        PropertyListQuery query,
        CancellationToken ct)
    {
        var builder = Builders<Property>.Filter;
        var filters = new List<FilterDefinition<Property>>();

        if (!string.IsNullOrWhiteSpace(query.City))
            filters.Add(builder.Eq(x => x.City, query.City.Trim()));

        if (query.Type.HasValue)
            filters.Add(builder.Eq(x => x.Type, query.Type.Value));

        if (query.Status.HasValue)
            filters.Add(builder.Eq(x => x.Status, query.Status.Value));

        if (query.MinPrice.HasValue)
            filters.Add(builder.Gte(x => x.Price, query.MinPrice.Value));

        if (query.MaxPrice.HasValue)
            filters.Add(builder.Lte(x => x.Price, query.MaxPrice.Value));

        if (query.BrokerId.HasValue)
            filters.Add(builder.Eq(x => x.BrokerId, query.BrokerId.Value));

        if (query.AgencyId.HasValue)
            filters.Add(builder.Eq(x => x.AgencyId, query.AgencyId.Value));

        var filter = filters.Count == 0 ? builder.Empty : builder.And(filters);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var totalItems = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection.Find(filter)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (items, totalItems);
    }
}