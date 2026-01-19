using RealEstate.TestData.Mongo;
using Xunit;

namespace RealEstate.Tests.Integration.Infrastructure;

[CollectionDefinition("MongoDb")]
public sealed class MongoDbCollection : ICollectionFixture<MongoDbFixture> { }