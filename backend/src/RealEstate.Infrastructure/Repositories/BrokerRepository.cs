using MongoDB.Driver;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories;

public sealed class BrokerRepository : IBrokerRepository
{
    private readonly IMongoCollection<Broker> _collection;

    public BrokerRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Broker>(MongoCollectionNames.Brokers);
    }

    // Equals to SQL SELECT ... WHERE Id = ... LIMIT 1
    public Task<Broker?> FindByIdAsync(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Broker>> GetAllAsync(CancellationToken ct) =>
        await _collection.Find(Builders<Broker>.Filter.Empty)
            .SortBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(ct);
    
    // INSERT
    public Task CreateAsync(Broker entity, CancellationToken ct) =>
        _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task<bool> UpdateAsync(Broker entity, CancellationToken ct)
    {
        var res = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: ct);
        return res.MatchedCount == 1 && res.ModifiedCount == 1;
    }

    // DELETE FROM ... WHERE Id = ...
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var res = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return res.DeletedCount == 1;
    }
}