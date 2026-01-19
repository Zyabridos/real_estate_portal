using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Properties;
using RealEstate.TestData;
using RealEstate.TestData.Mongo;
using RealEstate.Tests.Integration.Infrastructure;
using Xunit;

namespace RealEstate.Tests.Integration.Endpoints.Brokers;

[Collection("MongoDb")]
public sealed class BrokerPropertyLinkTests : IntegrationTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Broker> _brokers;
    private readonly IMongoCollection<Property> _properties;

    public BrokerPropertyLinkTests(MongoDbFixture fixture) : base(fixture)
    {
        _client = Ctx.Client;
        _brokers = Ctx.Collection<Broker>("brokers");
        _properties = Ctx.Collection<Property>("properties");
    }

    [Fact]
    public async Task Properties_list_filters_by_brokerId_and_returns_only_brokers_properties()
    {
        var brokerA = TestBrokers.Create(firstName: "Alice", lastName: "Agent");
        var brokerB = TestBrokers.Create(firstName: "Bob", lastName: "Broker");

        await _brokers.InsertManyAsync(new[] { brokerA, brokerB });

        var a1 = TestProperties.CreateForBroker(brokerA.Id, title: "A-1", type: PropertyType.Apartment, status: PropertyStatus.Active);
        var a2 = TestProperties.CreateForBroker(brokerA.Id, title: "A-2", type: PropertyType.Apartment, status: PropertyStatus.Active);
        var b1 = TestProperties.CreateForBroker(brokerB.Id, title: "B-1", type: PropertyType.Apartment, status: PropertyStatus.Active);

        await _properties.InsertManyAsync(new[] { a1, a2, b1 });

        var response = await _client.GetAsync($"/api/properties?brokerId={brokerA.Id}&page=1&pageSize=50");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<PropertyListItemDto>>();
        payload.Should().NotBeNull();

        var ids = payload!.Items.Select(x => x.Id).ToHashSet();
        ids.Should().Contain(a1.Id);
        ids.Should().Contain(a2.Id);
        ids.Should().NotContain(b1.Id);
        payload.Items.Should().HaveCount(2);
    }
}
