using MongoDB.Driver;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories;

public sealed class LeadRepository : ILeadRepository
{
    private readonly IMongoCollection<Lead> _collection;

    public LeadRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Lead>(MongoCollectionNames.Leads);
    }

    public Task<Lead?> FindByIdAsync(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public Task CreateAsync(Lead entity, CancellationToken ct) =>
        _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task<bool> UpdateStatusAsync(Guid id, LeadStatus status, CancellationToken ct)
    {
        var update = Builders<Lead>.Update.Set(x => x.Status, status);
        var res = await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: ct);
        return res.MatchedCount == 1 && res.ModifiedCount == 1;
    }

    public async Task<IReadOnlyList<Lead>> FindByPropertyIdAsync(Guid propertyId, int limit, CancellationToken ct)
    {
        var items = await _collection.Find(x => x.PropertyId == propertyId)
            .SortByDescending(x => x.CreatedAt)
            .Limit(limit)
            .ToListAsync(ct);

        return items;
    }
}