using RealEstate.TestData.Mongo;

namespace RealEstate.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase : MongoDbTestBase
{
    protected IntegrationTestContext Ctx { get; }

    protected IntegrationTestBase(MongoDbFixture fixture) : base(fixture)
    {
        Ctx = new IntegrationTestContext(fixture);
    }
}