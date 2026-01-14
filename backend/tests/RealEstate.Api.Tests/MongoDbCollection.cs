using RealEstate.Testing.Mongo;
using Xunit;

namespace RealEstate.Infrastructure.Tests;

[CollectionDefinition("MongoDb")]
public sealed class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
}

