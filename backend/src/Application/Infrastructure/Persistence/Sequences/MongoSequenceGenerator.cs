using MongoDB.Driver;
using RealEstate.Application.Common.Abstractions;

namespace RealEstate.Infrastructure.Persistence.Sequences;

public sealed class MongoSequenceGenerator : ISequenceGenerator
{
    private readonly IMongoCollection<SequenceCounter> _collection;

    public MongoSequenceGenerator(IMongoDatabase database)
    {
        _collection = database.GetCollection<SequenceCounter>("counters");
    }

    public async Task<int> GetNextValueAsync(string sequenceName, CancellationToken ct)
    {
        var filter = Builders<SequenceCounter>.Filter.Eq(x => x.Name, sequenceName);
        var update = Builders<SequenceCounter>.Update.Inc(x => x.Value, 1);

        var options = new FindOneAndUpdateOptions<SequenceCounter>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        var counter = await _collection.FindOneAndUpdateAsync(
            filter,
            update,
            options,
            ct);

        return counter.Value;
    }
}