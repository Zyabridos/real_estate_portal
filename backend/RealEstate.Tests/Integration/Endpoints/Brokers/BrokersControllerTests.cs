using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Brokers;
using RealEstate.Domain.Entities;
using RealEstate.Testing.Fixtures;
using RealEstate.Testing.Mongo;
using Xunit;

namespace Integration.Controllers;

[Collection("MongoDb")]
public sealed class BrokersControllerTests : MongoDbTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Broker> _brokers;

    public BrokersControllerTests(MongoDbFixture fixture) : base(fixture)
    {
        var factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        _client = factory.CreateClient();
        _brokers = Fixture.Database.GetCollection<Broker>("brokers");
    }

    [Fact]
    public async Task GetList_returns_200_and_paged_contract()
    {
        // Arrange
        await SeedBrokersAsync();

        // Act
        var response = await _client.GetAsync("/api/brokers?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<BrokerListItemDto>>();
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull();
        payload.Page.Should().Be(1);
        payload.PageSize.Should().Be(10);

        // Totals/Items sanity checks
        payload.TotalCount.Should().BeGreaterThan(1);
        payload.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_existing_returns_200()
    {
        // Arrange
        var brokerId = await SeedBrokerAsync(firstName: "Anna", lastName: "Test");

        // Act
        var response = await _client.GetAsync($"/api/brokers/{brokerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<BrokerDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(brokerId);
        dto.FirstName.Should().Be("Anna");
        dto.LastName.Should().Be("Test");
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        // Arrange
        var missingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/brokers/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task GetById_invalid_guid_returns_400(string rawId)
    {
        // Act
        var response = await _client.GetAsync($"/api/brokers/{rawId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task SeedBrokersAsync()
    {
        var now = DateTime.UtcNow;

        var brokers = new[]
        {
            new Broker
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+4711111111",
                PhotoUrl = null,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Broker
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Roe",
                Email = "jane.roe@example.com",
                PhoneNumber = "+4722222222",
                PhotoUrl = null,
                CreatedAt = now,
                UpdatedAt = now,
            }
        };

        await _brokers.InsertManyAsync(brokers);
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
            PhoneNumber = "+4733333333",
            PhotoUrl = null,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _brokers.InsertOneAsync(broker);
        return broker.Id;
    }
}
