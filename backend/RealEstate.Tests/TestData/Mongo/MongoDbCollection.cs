using RealEstate.TestData.Mongo;
using Xunit;

namespace RealEstate.TestData.Mongo;

[CollectionDefinition("MongoDb")]
public sealed class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
}