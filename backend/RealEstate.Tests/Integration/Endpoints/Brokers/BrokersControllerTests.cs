using System.Net;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Brokers.GetById;
using RealEstate.Application.Features.Brokers.List;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Brokers;
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
            TestBrokers.Create(firstName: "John", lastName: "Doe", gender: BrokerGender.Male),
            TestBrokers.Create(firstName: "Jane", lastName: "Roe", gender: BrokerGender.Female),
        });

        var response = await Ctx.Client.GetAsync("/api/brokers?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonTestAsync<PagedResult<BrokerListItemDto>>();
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull();
        payload.Page.Should().Be(1);
        payload.PageSize.Should().Be(10);
        payload.TotalItems.Should().BeGreaterThan(1);
        payload.Items.Count.Should().BeGreaterThan(0);
        payload.Items.Should().Contain(x => x.Gender == BrokerGender.Male);
        payload.Items.Should().Contain(x => x.Gender == BrokerGender.Female);
    }

    [Fact]
    public async Task GetById_existing_returns_200()
    {
        var broker = TestBrokers.Create(
            firstName: "Anna",
            lastName: "Test",
            gender: BrokerGender.Female);

        await _brokers.InsertOneAsync(broker);

        var response = await Ctx.Client.GetAsync($"/api/brokers/{broker.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonTestAsync<BrokerDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(broker.Id);
        dto.FirstName.Should().Be("Anna");
        dto.LastName.Should().Be("Test");
        dto.Gender.Should().Be(BrokerGender.Female);
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        var response = await Ctx.Client.GetAsync("/api/brokers/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-an-int")]
    public async Task GetById_invalid_id_returns_400(string rawId)
    {
        var response = await Ctx.Client.GetAsync($"/api/brokers/{rawId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}