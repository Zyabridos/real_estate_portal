using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Mongo.Indexes;

public sealed class MongoIndexInitializer : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MongoIndexInitializer> _logger;

    public MongoIndexInitializer(IServiceScopeFactory scopeFactory, ILogger<MongoIndexInitializer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ensuring MongoDB indexes...");

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

        var agencies = db.GetCollection<Agency>(MongoCollectionNames.Agencies);
        var properties = db.GetCollection<Property>(MongoCollectionNames.Properties);
        var leads = db.GetCollection<Lead>(MongoCollectionNames.Leads);
        
        // Agencies
        await agencies.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Agency>(Builders<Agency>.IndexKeys.Ascending(x => x.Id)),
            new CreateIndexModel<Agency>(Builders<Agency>.IndexKeys.Ascending(x => x.OrgNumber)),
            new CreateIndexModel<Agency>(Builders<Agency>.IndexKeys.Descending(x => x.CreatedAt)),
            new CreateIndexModel<Agency>(Builders<Agency>.IndexKeys
                .Ascending(x => x.Id)
                .Descending(x => x.CreatedAt))
        }, cancellationToken);

        // Properties
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

        // Leads
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
