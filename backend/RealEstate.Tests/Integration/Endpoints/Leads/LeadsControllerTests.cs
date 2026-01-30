using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;
using RealEstate.Domain.Entities;
using RealEstate.TestData;
using RealEstate.TestData.Fixtures;
using RealEstate.TestData.Mongo;
using RealEstate.Api.Tests.Integration;
using RealEstate.Tests.Integration.Infrastructure;
using Xunit;

namespace RealEstate.Api.Tests.Integration.Endpoints.Leads;

[Collection("MongoDb")]
public sealed class LeadsControllerTests : IntegrationTestBase
{
    private readonly IMongoCollection<Lead> _leads;

    public LeadsControllerTests(MongoDbFixture fixture) : base(fixture)
    {
        _leads = Ctx.Collection<Lead>("leads");
    }

    [Fact]
    public async Task GetList_returns_200_and_paged_contract()
    {
        var propertyId = Guid.NewGuid();

        await _leads.InsertManyAsync(new[]
        {
            TestLeads.Create(propertyId, fullName: "Cercei Lannister", email: "cercei@example.com"),
            TestLeads.Create(propertyId, fullName: "Jane Roe", email: "jane.roe@example.com"),
        });

        var response = await Ctx.Client.GetAsync("/api/leads?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonTestAsync<PagedResult<LeadListItemDto>>();
        payload.Should().NotBeNull();
        payload!.Items.Should().NotBeNull();
        payload.Page.Should().Be(1);
        payload.PageSize.Should().Be(10);
        payload.TotalItems.Should().BeGreaterThan(1);
        payload.Items.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_existing_returns_200()
    {
        var lead = TestLeads.Create(Guid.NewGuid(), fullName: "Anna Test", email: "anna.test@example.com");
        await _leads.InsertOneAsync(lead);

        var response = await Ctx.Client.GetAsync($"/api/leads/{lead.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonTestAsync<LeadDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(lead.Id);
        dto.FullName.Should().Be("Anna Test");
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        var response = await Ctx.Client.GetAsync($"/api/leads/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task GetById_invalid_guid_returns_400(string rawId)
    {
        var response = await Ctx.Client.GetAsync($"/api/leads/{rawId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
