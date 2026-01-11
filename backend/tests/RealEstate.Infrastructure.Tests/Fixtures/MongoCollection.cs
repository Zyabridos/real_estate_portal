using Xunit;

namespace RealEstate.Infrastructure.Tests.Fixtures;

[CollectionDefinition("MongoDb")]
public sealed class MongoCollection : ICollectionFixture<MongoDbFixture> { }