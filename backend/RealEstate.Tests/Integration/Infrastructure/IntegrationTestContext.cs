using System;
using System.Net.Http;
using MongoDB.Driver;
using RealEstate.TestData.Fixtures;
using RealEstate.TestData.Mongo;

namespace RealEstate.Tests.Integration.Infrastructure;

public sealed class IntegrationTestContext
{
    public MongoDbFixture Fixture { get; }
    public HttpClient Client { get; }

    public IntegrationTestContext(MongoDbFixture fixture)
    {
        Fixture = fixture;

        // Ensure Mongo settings are available early - otherwise tests might not "catch" the DB
        if (string.IsNullOrWhiteSpace(fixture.ConnectionString))
            throw new InvalidOperationException("MongoDbFixture.ConnectionString is empty.");

        if (string.IsNullOrWhiteSpace(fixture.DatabaseName))
            throw new InvalidOperationException("MongoDbFixture.DatabaseName is empty.");

        Environment.SetEnvironmentVariable("Mongo__ConnectionString", fixture.ConnectionString);
        Environment.SetEnvironmentVariable("Mongo__Database", fixture.DatabaseName);
        Environment.SetEnvironmentVariable("Mongo__DatabaseName", fixture.DatabaseName);

        var factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        Client = factory.CreateClient();
    }

    public IMongoCollection<T> Collection<T>(string name) =>
        Fixture.Database.GetCollection<T>(name);

    public IMongoCollection<T> GetCollection<T>(string name) =>
        Collection<T>(name);
}