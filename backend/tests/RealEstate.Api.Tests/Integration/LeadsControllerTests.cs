using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.DTOs.Leads;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Testing.Fixtures;
using RealEstate.Testing.Mongo;
using Xunit;

namespace RealEstate.Api.Tests.Integration;

[Collection("MongoDb")]
public sealed class LeadsControllerTests : MongoDbTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Lead> _leads;

    public LeadsControllerTests(MongoDbFixture fixture) : base(fixture)
    {
        var factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        _client = factory.CreateClient();
        _leads = Fixture.Database.GetCollection<Lead>("leads");
    }

    [Fact]
    public async Task GetList_returns_200_and_paged_contract()
    {
        // Arrange
        await SeedLeadsAsync();

        // Act
        var response = await _client.GetAsync("/api/leads?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<LeadListItemDto>>();
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
        // Arrange
        var leadId = await SeedLeadAsync(fullName: "Anna Test");

        // Act
        var response = await _client.GetAsync($"/api/leads/{leadId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<LeadDetailsDto>();
        dto.Should().NotBeNull();

        dto!.Id.Should().Be(leadId);
        dto.FullName.Should().Be("Anna Test");
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        // Arrange
        var missingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/leads/{missingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task GetById_invalid_guid_returns_400(string rawId)
    {
        // Act
        var response = await _client.GetAsync($"/api/leads/{rawId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task SeedLeadsAsync()
    {
        var now = DateTime.UtcNow;
        var propertyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var leads = new[]
        {
            new Lead
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                FullName = "Cercei Lannister",
                Email = "cercei.lannister@greatlannisters.com",
                PhoneNumber = "+4711111111",
                Message = "Interested",
                Status = LeadStatus.New,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Lead
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                FullName = "Jane Roe",
                Email = "jane.roe@example.com",
                PhoneNumber = "+4722222222",
                Message = null,
                Status = LeadStatus.New,
                CreatedAt = now,
                UpdatedAt = now,
            }
        };

        await _leads.InsertManyAsync(leads);
    }

    private async Task<Guid> SeedLeadAsync(string fullName)
    {
        var now = DateTime.UtcNow;

        var lead = new Lead
        {
            Id = Guid.NewGuid(),
            PropertyId = Guid.NewGuid(),
            FullName = fullName,
            Email = $"{fullName.ToLowerInvariant().Replace(" ", ".")}@example.com",
            PhoneNumber = "+4733333333",
            Message = "Hello",
            Status = LeadStatus.New,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _leads.InsertOneAsync(lead);
        return lead.Id;
    }
}