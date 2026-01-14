using Xunit;

namespace RealEstate.Api.Tests.Fixtures;

[CollectionDefinition("MongoDb")]
public sealed class MongoCollection : ICollectionFixture<MongoDbFixture> { }