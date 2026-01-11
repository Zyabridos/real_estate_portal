using RealEstate.Infrastructure.Mongo.Conventions;

namespace RealEstate.Infrastructure.Tests.Fixtures;

public abstract class MongoDbTestBase : IAsyncLifetime
{
    protected readonly MongoDbFixture Fixture;

    protected MongoDbTestBase(MongoDbFixture fixture)
    {
        Fixture = fixture;
        MongoConventions.Register();
    }

    public Task InitializeAsync() => Fixture.ClearDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;
}