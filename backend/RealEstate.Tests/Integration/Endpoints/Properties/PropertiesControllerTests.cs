using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Testing.Fixtures; 
using RealEstate.Testing.Mongo;
using RealEstate.Testing.TestData;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using Xunit;

namespace Integration.Controllers;

[Collection("MongoDb")]
public sealed class PropertiesControllerTests : MongoDbTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Property> _properties;

    public PropertiesControllerTests(MongoDbFixture fixture) : base(fixture)
    {
        var factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        _client = factory.CreateClient();

        _properties = Fixture.Database.GetCollection<Property>("properties");
    }

    [Fact]
    public async Task List_empty_returns_empty_items_and_total_0()
    {
        var resp = await _client.GetAsync("/api/properties?page=1&pageSize=10");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<PagedResult<PropertyListItemDto>>();
        body.Should().NotBeNull();

        body!.Items.Should().NotBeNull();
        body.Items.Should().BeEmpty();
        body.TotalCount.Should().Be(0);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task List_with_filters_returns_only_matching_items()
    {
        var now = DateTime.UtcNow;

        var seed = new[]
        {
            TestProperties.Create(city: "Trondheim", price: 4_000_000m, type: PropertyType.Apartment, status: PropertyStatus.Active, createdAt: now.AddMinutes(-1)),
            TestProperties.Create(city: "Trondheim", price: 8_000_000m, type: PropertyType.House, status: PropertyStatus.Active, createdAt: now.AddMinutes(-2)),
            TestProperties.Create(city: "Oslo",     price: 4_500_000m, type: PropertyType.Apartment, status: PropertyStatus.Active, createdAt: now.AddMinutes(-3)),
            TestProperties.Create(city: "Trondheim", price: 3_500_000m, type: PropertyType.Apartment, status: PropertyStatus.Sold, createdAt: now.AddMinutes(-4)),
            TestProperties.Create(city: "Trondheim", price: 4_600_000m, type: PropertyType.Apartment, status: PropertyStatus.Active, createdAt: now.AddMinutes(-5)),
        };

        await _properties.InsertManyAsync(seed);

        // Match:
        // city=Trondheim
        // type=Apartment
        // status=Active
        // minPrice=4_000_000
        // maxPrice=5_000_000
        var url =
            "/api/properties" +
            "?city=Trondheim" +
            "&type=Apartment" +
            "&status=Active" +
            "&minPrice=4000000" +
            "&maxPrice=5000000" +
            "&page=1&pageSize=20";

        var resp = await _client.GetAsync(url);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<PagedResult<PropertyListItemDto>>();
        body.Should().NotBeNull();

        body!.TotalCount.Should().BeGreaterThan(0);
        body.Items.Should().NotBeEmpty();

        body.Items.Should().OnlyContain(x =>
            x.City == "Trondheim" &&
            x.Type == PropertyType.Apartment &&
            x.Status == PropertyStatus.Active &&
            x.Price >= 4_000_000m &&
            x.Price <= 5_000_000m
        );
    }

    [Fact]
    public async Task Paging_works_page_2_size_10_returns_10_items_and_metadata()
    {
        var now = DateTime.UtcNow;

        var seed = Enumerable.Range(1, 25)
            .Select(i => TestProperties.Create(
                title: $"Property {i}",
                createdAt: now.AddMinutes(-i)))
            .ToList();

        await _properties.InsertManyAsync(seed);

        var resp = await _client.GetAsync("/api/properties?page=2&pageSize=10&sort=createdAtDesc");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<PagedResult<PropertyListItemDto>>();
        body.Should().NotBeNull();

        body!.Items.Should().HaveCount(10);
        body.TotalCount.Should().Be(25);
        body.Page.Should().Be(2);
        body.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Details_non_existing_id_returns_404()
    {
        var id = Guid.NewGuid();
        var resp = await _client.GetAsync($"/api/properties/{id}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
