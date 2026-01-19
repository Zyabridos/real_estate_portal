using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Brokers;
using RealEstate.Domain.Entities;
using RealEstate.TestData;
using RealEstate.TestData.Fixtures;
using RealEstate.TestData.Mongo;
using RealEstate.Api.Tests.Integration;
using RealEstate.Tests.Integration.Infrastructure;
using Xunit;

namespace RealEstate.Api.Tests.Integration.Endpoints.Brokers;

[Collection("MongoDb")]
public sealed class BrokersControllerTests : IntegrationTestBase
{
    private readonly IMongoCollection<Broker> _brokers;

    public BrokersControllerTests(MongoDbFixture fixture) : base(fixture)
    {
        _brokers = Ctx.Collection<Broker>("brokers");
    }

    [Fact]
    public async Task GetList_returns_200_and_paged_contract()
    {
        await _brokers.InsertManyAsync(new[]
        {
            TestBrokers.Create(firstName: "John", lastName: "Doe"),
            TestBrokers.Create(firstName: "Jane", lastName: "Roe"),
        });

        var response = await Ctx.Client.GetAsync("/api/brokers?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<BrokerListItemDto>>();
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull();
        payload.Page.Should().Be(1);
        payload.PageSize.Should().Be(10);
        payload.TotalCount.Should().BeGreaterThan(1);
        payload.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_existing_returns_200()
    {
        var broker = TestBrokers.Create(firstName: "Anna", lastName: "Test");
        await _brokers.InsertOneAsync(broker);

        var response = await Ctx.Client.GetAsync($"/api/brokers/{broker.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<BrokerDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(broker.Id);
        dto.FirstName.Should().Be("Anna");
        dto.LastName.Should().Be("Test");
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        var response = await Ctx.Client.GetAsync($"/api/brokers/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task GetById_invalid_guid_returns_400(string rawId)
    {
        var response = await Ctx.Client.GetAsync($"/api/brokers/{rawId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
