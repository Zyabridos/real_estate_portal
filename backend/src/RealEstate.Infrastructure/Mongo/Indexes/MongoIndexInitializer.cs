using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Mongo.Indexes;

public sealed class MongoIndexInitializer : IHostedService
{
    private readonly IMongoDatabase _db;
    private readonly ILogger<MongoIndexInitializer> _logger;

    public MongoIndexInitializer(IMongoDatabase db, ILogger<MongoIndexInitializer> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ensuring MongoDB indexes...");

        var properties = _db.GetCollection<Property>(MongoCollectionNames.Properties);
        var leads = _db.GetCollection<Lead>(MongoCollectionNames.Leads);

        // Properties: brokerId, city, type, status + compound city+type
        await properties.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(x => x.BrokerId)),
            new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(x => x.City)),
            new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(x => x.Type)),
            new CreateIndexModel<Property>(Builders<Property>.IndexKeys.Ascending(x => x.Status)),
            new CreateIndexModel<Property>(Builders<Property>.IndexKeys
                .Ascending(x => x.City)
                .Ascending(x => x.Type))
        }, cancellationToken);

        // Leads: propertyId, createdAt, status + compound propertyId+createdAt
        await leads.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Lead>(Builders<Lead>.IndexKeys.Ascending(x => x.PropertyId)),
            new CreateIndexModel<Lead>(Builders<Lead>.IndexKeys.Descending(x => x.CreatedAt)),
            new CreateIndexModel<Lead>(Builders<Lead>.IndexKeys.Ascending(x => x.Status)),
            new CreateIndexModel<Lead>(Builders<Lead>.IndexKeys
                .Ascending(x => x.PropertyId)
                .Descending(x => x.CreatedAt))
        }, cancellationToken);

        _logger.LogInformation("MongoDB indexes ensured.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
