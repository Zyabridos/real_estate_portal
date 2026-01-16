using Xunit;
using RealEstate.Testing.Mongo;

namespace RealEstate.Api.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<MongoDbFixture>, IAsyncLifetime
{
    protected MongoDbFixture Mongo { get; }

    protected IntegrationTestBase(MongoDbFixture mongo)
    {
        Mongo = mongo;
    }

    // xUnit creates new test-class on each test
    // easier comparasment : InitializeAsync = "beforeEach" in Vitest/Jest
    public async Task InitializeAsync()
    {
        await Mongo.ClearDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}