using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.DTOs.Leads;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Testing.Fixtures;
using RealEstate.Testing.Mongo;
using RealEstate.Testing.TestData;
using Xunit;

namespace RealEstate.Api.Tests.Integration;

[Collection("MongoDb")]
public sealed class LinkBetweebLeadAndPropertiesTests : MongoDbTestBase
{
    private readonly HttpClient _client;
    private readonly IMongoCollection<Property> _properties;
    private readonly IMongoCollection<Lead> _leads;

    public LinkBetweebLeadAndPropertiesTests(MongoDbFixture fixture) : base(fixture)
    {
        var factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        _client = factory.CreateClient();

        _properties = Fixture.Database.GetCollection<Property>("properties");
        _leads = Fixture.Database.GetCollection<Lead>("leads");
    }

    [Fact]
    public async Task CreateLead_with_existing_property_links_lead_to_property_in_response_and_db()
    {
        // Arrange: seed a property that lead will reference
        var property = TestProperties.Create(
            city: "Trondheim",
            price: 4_200_000m,
            type: PropertyType.Apartment,
            status: PropertyStatus.Active,
            createdAt: DateTime.UtcNow.AddMinutes(-1)
        );

        await _properties.InsertOneAsync(property);

        var request = new CreateLeadRequest(
            PropertyId: property.Id,
            FullName: "Link Test",
            Email: "link.test@example.com",
            PhoneNumber: null,
            Message: "Please contact me"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/leads", request);

        // Assert: 201 and response contains correct PropertyId
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<LeadDetailsDto>();
        created.Should().NotBeNull();

        created!.PropertyId.Should().Be(property.Id);

        // Assert: lead is persisted and PropertyId matches in MongoDB
        var saved = await _leads.Find(x => x.Id == created.Id).FirstOrDefaultAsync();
        saved.Should().NotBeNull();

        saved!.PropertyId.Should().Be(property.Id);

        // Optional sanity: property exists via API (proves link points to a real object)
        var propertyResp = await _client.GetAsync($"/api/properties/{property.Id}");
        propertyResp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
