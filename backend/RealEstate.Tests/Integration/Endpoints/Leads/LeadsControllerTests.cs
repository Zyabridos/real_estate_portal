using System.Net;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Domain.Entities;
using RealEstate.TestData;
using RealEstate.TestData.Mongo;
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
        const int propertyId = 100;
        const int agencyId = 10;
        const int brokerId = 20;

        await _leads.InsertManyAsync(new[]
        {
            TestLeads.Create(
                id: 1,
                agencyId: agencyId,
                brokerId: brokerId,
                propertyId: propertyId,
                fullName: "Cercei Lannister",
                email: "cercei@example.com"
            ),
            TestLeads.Create(
                id: 2,
                agencyId: agencyId,
                brokerId: brokerId,
                propertyId: propertyId,
                fullName: "Jane Roe",
                email: "jane.roe@example.com"
            ),
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
        var lead = TestLeads.Create(
            id: 1,
            agencyId: 10,
            brokerId: 20,
            propertyId: 100,
            fullName: "Anna Test",
            email: "anna.test@example.com"
        );

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
        var response = await Ctx.Client.GetAsync("/api/leads/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-an-int")]
    public async Task GetById_invalid_id_returns_400(string rawId)
    {
        var response = await Ctx.Client.GetAsync($"/api/leads/{rawId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}