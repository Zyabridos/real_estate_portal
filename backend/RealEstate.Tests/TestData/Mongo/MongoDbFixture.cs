using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace RealEstate.TestData.Mongo;

public sealed class MongoDbFixture : IAsyncLifetime
{
    private MongoDbContainer _container = default!;

    public IMongoClient Client { get; private set; } = default!;
    public IMongoDatabase Database { get; private set; } = default!;

    public string ConnectionString { get; private set; } = default!;
    public string DatabaseName { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        _container = new MongoDbBuilder()
            .WithImage("mongo:7")
            .Build();

        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString();
        DatabaseName = $"realestate_api_tests_{Guid.NewGuid():N}";

        Client = new MongoClient(ConnectionString);
        Database = Client.GetDatabase(DatabaseName);
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    public async Task ClearDatabaseAsync()
    {
        var collections = await Database.ListCollectionNames().ToListAsync();
        foreach (var name in collections)
        {
            await Database.DropCollectionAsync(name);
        }
    }
}