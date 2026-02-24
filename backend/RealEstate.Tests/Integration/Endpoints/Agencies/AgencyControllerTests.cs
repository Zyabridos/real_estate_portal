using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MongoDB.Driver;
using RealEstate.Application.Common;
using RealEstate.Application.Features.Agencies.Create;
using RealEstate.Application.Features.Agencies.GetById;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Application.Features.Agencies.Update;
using RealEstate.Domain.Entities;
using RealEstate.TestData;
using RealEstate.TestData.Fixtures;
using RealEstate.TestData.Mongo;
using RealEstate.Api.Tests.Integration;
using RealEstate.Tests.Integration.Infrastructure;
using Xunit;

namespace RealEstate.Api.Tests.Integration.Endpoints.Agencies;

[Collection("MongoDb")]
public sealed class AgenciesControllerTests : IntegrationTestBase
{
    private readonly IMongoCollection<Agency> _agencies;

    public AgenciesControllerTests(MongoDbFixture fixture) : base(fixture)
    {
        _agencies = Ctx.Collection<Agency>("agencies");
    }

    [Fact]
    public async Task GetList_returns_200_and_paged_contract()
    {
        await _agencies.InsertManyAsync(new[]
        {
            new Agency { Id = Guid.NewGuid(), Name = "A", OrgNumber = "111111", PhoneNumber = "+47 1", City = "Trondheim", Street = "S1", ZipCode = "7010", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Agency { Id = Guid.NewGuid(), Name = "B", OrgNumber = "222222", PhoneNumber = "+47 2", City = "Oslo", Street = "S2", ZipCode = "0001", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        });

        var response = await Ctx.Client.GetAsync("/api/agencies?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<AgencyListItemDto>>();
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
        var agency = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "Test Agency",
            OrgNumber = "123123123",
            PhoneNumber = "+4711111111",
            City = "Trondheim",
            Street = "Testgata 1",
            ZipCode = "7010",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _agencies.InsertOneAsync(agency);

        var response = await Ctx.Client.GetAsync($"/api/agencies/{agency.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<AgencyDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(agency.Id);
        dto.Name.Should().Be("Test Agency");
        dto.OrgNumber.Should().Be("123123123");
    }

    [Fact]
    public async Task GetById_missing_returns_404()
    {
        var response = await Ctx.Client.GetAsync($"/api/agencies/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task GetById_invalid_guid_returns_400(string rawId)
    {
        var response = await Ctx.Client.GetAsync($"/api/agencies/{rawId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_returns_201_and_GetById_returns_created()
    {
        var request = new CreateAgencyRequest(
            Name: "Created Agency",
            OrgNumber: "999999",
            PhoneNumber: " +47 111 22 333 ",
            City: "Trondheim",
            Street: "NewStreet 1",
            ZipCode: "7010"
        );

        var createResponse = await Ctx.Client.PostAsJsonAsync("/api/agencies", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<AgencyDetailsDto>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.Name.Should().Be("Created Agency");
        created.OrgNumber.Should().Be("999999");
        
        created.PhoneNumber.Should().Be("+4711122333");

        var getResponse = await Ctx.Client.GetAsync($"/api/agencies/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<AgencyDetailsDto>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be("Created Agency");
        fetched.PhoneNumber.Should().Be("+4711122333");
    }

    [Fact]
    public async Task Update_existing_returns_200_and_updates_fields()
    {
        var existing = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            OrgNumber = "111111",
            PhoneNumber = "+47 999 99 999",
            City = "OldCity",
            Street = "OldStreet",
            ZipCode = "0000",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        await _agencies.InsertOneAsync(existing);

        var request = new UpdateAgencyRequest(
            Id: existing.Id,
            Name: "New Name",
            PhoneNumber: " +47 111 22 333 ",
            City: "Trondheim",
            Street: "NewStreet 2",
            ZipCode: "7010"
        );

        var response = await Ctx.Client.PutAsJsonAsync($"/api/agencies/{existing.Id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<AgencyDetailsDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(existing.Id);
        dto.Name.Should().Be("New Name");
        dto.PhoneNumber.Should().Be("+4711122333");

        var fromDb = await _agencies.Find(x => x.Id == existing.Id).FirstOrDefaultAsync();
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("New Name");
        fromDb.PhoneNumber.Should().Be("+4711122333");
        fromDb.City.Should().Be("Trondheim");
    }

    [Fact]
    public async Task Update_missing_returns_404()
    {
        var id = Guid.NewGuid();

        var request = new UpdateAgencyRequest(
            Id: id,
            Name: "Name",
            PhoneNumber: "+47 111 11 111",
            City: "City",
            Street: "Street",
            ZipCode: "Zip"
        );

        var response = await Ctx.Client.PutAsJsonAsync($"/api/agencies/{id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task Update_invalid_guid_returns_400(string rawId)
    {
        var request = new UpdateAgencyRequest(
            Id: Guid.NewGuid(),
            Name: "Name",
            PhoneNumber: "+47 111 11 111",
            City: "City",
            Street: "Street",
            ZipCode: "Zip"
        );

        var response = await Ctx.Client.PutAsJsonAsync($"/api/agencies/{rawId}", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_existing_returns_204_and_entity_is_gone()
    {
        var agency = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            OrgNumber = "333333",
            PhoneNumber = "+47 3",
            City = "Trondheim",
            Street = "S",
            ZipCode = "7010",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _agencies.InsertOneAsync(agency);

        var response = await Ctx.Client.DeleteAsync($"/api/agencies/{agency.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await Ctx.Client.GetAsync($"/api/agencies/{agency.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_missing_returns_404()
    {
        var response = await Ctx.Client.DeleteAsync($"/api/agencies/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("not-a-guid")]
    public async Task Delete_invalid_guid_returns_400(string rawId)
    {
        var response = await Ctx.Client.DeleteAsync($"/api/agencies/{rawId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}