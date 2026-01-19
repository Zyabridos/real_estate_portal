using RealEstate.Infrastructure.Mongo.Conventions;
using Xunit;

namespace RealEstate.TestData.Mongo;

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