using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Testing.Fixtures;
using RealEstate.Testing.Mongo;
using Xunit;

namespace RealEstate.Api.Tests.Integration;

[Collection("MongoDb")]
public sealed class LinkBetweenBrokerAndPropertiesTests : MongoDbTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Broker> _brokers;
    private readonly IMongoCollection<Property> _properties;

    public LinkBetweenBrokerAndPropertiesTests(MongoDbFixture fixture) : base(fixture)
    {
        var factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        _client = factory.CreateClient();
        _brokers = Fixture.Database.GetCollection<Broker>("brokers");
        _properties = Fixture.Database.GetCollection<Property>("properties");
    }

    [Fact]
    public async Task Properties_list_filters_by_brokerId_and_returns_only_brokers_properties()
    {
        // Arrange
        var brokerA = await SeedBrokerAsync("Alice", "Agent");
        var brokerB = await SeedBrokerAsync("Bob", "Broker");

        // Two properties for A, one for B
        var a1 = await SeedPropertyAsync(brokerA, title: "A-1");
        var a2 = await SeedPropertyAsync(brokerA, title: "A-2");
        var b1 = await SeedPropertyAsync(brokerB, title: "B-1");

        // Act
        var response = await _client.GetAsync($"/api/properties?brokerId={brokerA}&page=1&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<PropertyListItemDto>>();
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull();

        var resultIds = payload.Items.Select(x => x.Id).ToHashSet();

        resultIds.Should().Contain(a1);
        resultIds.Should().Contain(a2);
        resultIds.Should().NotContain(b1);
        
        payload.Items.Should().HaveCount(2);

        // Contains A properties, does not contain B property
        var ids = payload.Items.Select(x => x.Id).ToHashSet();
        ids.Should().Contain(a1);
        ids.Should().Contain(a2);
        ids.Should().NotContain(b1);
    }

    private async Task<Guid> SeedBrokerAsync(string firstName, string lastName)
    {
        var now = DateTime.UtcNow;

        var broker = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}@example.com",
            PhoneNumber = "+4744444444",
            PhotoUrl = null,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _brokers.InsertOneAsync(broker);
        return broker.Id;
    }

    private async Task<Guid> SeedPropertyAsync(Guid brokerId, string title)
    {
        var now = DateTime.UtcNow;

        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = "Test description",
            Address = "Test street 1",
            City = "Trondheim",
            Price = 1_000_000m,
            Type = PropertyType.Apartment,
            Bedrooms = 2,
            Bathrooms = 1,
            Area = 60,
            BrokerId = brokerId,
            Status = PropertyStatus.Active,
            MainImageUrl = null,
            CreatedAt = now
        };

        await _properties.InsertOneAsync(property);
        return property.Id;
    }
}
