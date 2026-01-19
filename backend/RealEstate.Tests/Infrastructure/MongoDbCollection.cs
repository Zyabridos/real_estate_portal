using Xunit;
using RealEstate.TestData.Mongo;

namespace RealEstate.Infrastructure.Tests.Infrastructure;

[CollectionDefinition("MongoDb")]
public sealed class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
}