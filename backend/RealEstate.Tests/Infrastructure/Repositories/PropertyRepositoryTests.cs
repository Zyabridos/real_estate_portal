using Xunit;
using FluentAssertions;
using RealEstate.Infrastructure.Repositories.Properties;
using RealEstate.Application.Features.Properties.List;
using RealEstate.Domain.Enums.Properties;
using RealEstate.TestData;
using RealEstate.TestData.Mongo;

namespace RealEstate.Infrastructure.Tests.Repositories;

[Collection("MongoDb")]
public sealed class PropertyRepositoryTests : MongoDbTestBase
{
    public PropertyRepositoryTests(MongoDbFixture fixture) : base(fixture)
    {
    }

    private PropertyRepository CreateRepo() => new(Fixture.Database);

    [Fact]
    public async Task Create_then_GetById_returns_entity()
    {
        var repo = CreateRepo();

        const int id = 1001;
        const int agencyId = 10;
        const int brokerId = 101;

        var entity = TestProperties.Create(
            id: id,
            agencyId: agencyId,
            brokerId: brokerId,
            title: "Nice apartment",
            city: "Trondheim",
            price: 4_500_000m,
            type: PropertyType.Apartment,
            status: PropertyStatus.Active,
            createdAt: DateTime.UtcNow
        );

        await repo.CreateAsync(entity, CancellationToken.None);

        var found = await repo.GetByIdAsync(id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(id);
        found.AgencyId.Should().Be(agencyId);
        found.BrokerId.Should().Be(brokerId);
        found.City.Should().Be("Trondheim");
        found.Type.Should().Be(PropertyType.Apartment);
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated_entity()
    {
        var repo = CreateRepo();

        var entity = TestProperties.Create(
            id: 1002,
            agencyId: 10,
            brokerId: 101,
            title: "Old title",
            city: "Oslo",
            price: 12_000_000m,
            type: PropertyType.House,
            status: PropertyStatus.Active,
            createdAt: DateTime.UtcNow
        );

        await repo.CreateAsync(entity, CancellationToken.None);

        entity.Title = "New title";

        var updated = await repo.UpdateAsync(entity, CancellationToken.None);
        updated.Should().BeTrue();

        var found = await repo.GetByIdAsync(entity.Id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.Title.Should().Be("New title");
        found.AgencyId.Should().Be(10);
        found.BrokerId.Should().Be(101);
    }

    [Fact]
    public async Task GetList_filters_by_city_and_type()
    {
        var repo = CreateRepo();

        const int agencyId = 10;
        const int brokerId = 101;
        var now = DateTime.UtcNow;

        await repo.CreateAsync(TestProperties.Create(
            id: 1003,
            agencyId: agencyId,
            brokerId: brokerId,
            title: "A",
            city: "Trondheim",
            price: 3_000_000m,
            type: PropertyType.Apartment,
            status: PropertyStatus.Active,
            createdAt: now.AddMinutes(-1)
        ), CancellationToken.None);

        await repo.CreateAsync(TestProperties.Create(
            id: 1004,
            agencyId: agencyId,
            brokerId: brokerId,
            title: "B",
            city: "Trondheim",
            price: 6_000_000m,
            type: PropertyType.House,
            status: PropertyStatus.Active,
            createdAt: now.AddMinutes(-2)
        ), CancellationToken.None);

        var query = new PropertyListQuery(
            City: "Trondheim",
            Type: PropertyType.Apartment,
            Status: null,
            MinPrice: null,
            MaxPrice: null,
            AgencyId: null,
            BrokerId: null,
            Page: 1,
            PageSize: 10,
            Sort: "createdAtDesc"
        );

        var (items, total) = await repo.GetListAsync(query, CancellationToken.None);

        total.Should().Be(1);
        items.Should().HaveCount(1);
        items[0].Type.Should().Be(PropertyType.Apartment);
        items[0].AgencyId.Should().Be(agencyId);
        items[0].BrokerId.Should().Be(brokerId);
    }

    [Fact]
    public async Task GetByAgencyBrokerAndId_returns_only_matching_property()
    {
        var repo = CreateRepo();

        var matching = TestProperties.Create(
            id: 2001,
            agencyId: 10,
            brokerId: 101,
            title: "Match"
        );

        var otherAgency = TestProperties.Create(
            id: 2002,
            agencyId: 11,
            brokerId: 101,
            title: "Other agency"
        );

        var otherBroker = TestProperties.Create(
            id: 2003,
            agencyId: 10,
            brokerId: 102,
            title: "Other broker"
        );

        await repo.CreateAsync(matching, CancellationToken.None);
        await repo.CreateAsync(otherAgency, CancellationToken.None);
        await repo.CreateAsync(otherBroker, CancellationToken.None);

        var found = await repo.GetByAgencyBrokerAndIdAsync(10, 101, 2001, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(2001);
        found.AgencyId.Should().Be(10);
        found.BrokerId.Should().Be(101);
    }
}