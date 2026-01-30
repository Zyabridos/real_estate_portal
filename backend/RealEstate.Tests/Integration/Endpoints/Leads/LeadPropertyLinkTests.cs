using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Properties;
using RealEstate.TestData;
using RealEstate.TestData.Mongo;
using RealEstate.TestData.Requests;
using RealEstate.Tests.Integration.Infrastructure;
using Xunit;

namespace RealEstate.Tests.Integration.Endpoints.Leads;

[Collection("MongoDb")]
public sealed class LeadPropertyLinkTests : IntegrationTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Property> _properties;
    private readonly IMongoCollection<Lead> _leads;

    public LeadPropertyLinkTests(MongoDbFixture fixture) : base(fixture)
    {
        _client = Ctx.Client;
        _properties = Ctx.Collection<Property>("properties");
        _leads = Ctx.Collection<Lead>("leads");
    }

    [Fact]
    public async Task CreateLead_with_existing_property_links_lead_to_property_in_response_and_db()
    {
        var property = TestProperties.Create(
            city: "Trondheim",
            price: 4_200_000m,
            type: PropertyType.Apartment,
            status: PropertyStatus.Active,
            createdAt: DateTime.UtcNow.AddMinutes(-1)
        );

        await _properties.InsertOneAsync(property);

        var request = TestLeadRequests.Valid(property.Id);

        var response = await _client.PostAsJsonAsync("/api/leads", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonTestAsync<LeadDetailsDto>();
        created.Should().NotBeNull();
        created!.PropertyId.Should().Be(property.Id);

        var saved = await _leads.Find(x => x.Id == created.Id).FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.PropertyId.Should().Be(property.Id);
    }
}