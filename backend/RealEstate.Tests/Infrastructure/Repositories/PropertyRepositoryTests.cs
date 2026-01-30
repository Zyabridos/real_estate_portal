using Xunit;
using FluentAssertions;
using RealEstate.Infrastructure.Repositories.Properties;

using RealEstate.Application.Features.Properties.List;
using RealEstate.Domain.Enums.Properties;
using RealEstate.Domain.Entities;

using RealEstate.Infrastructure.Repositories;

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

        var id = Guid.NewGuid();
        var brokerId = Guid.NewGuid();

        var entity = TestProperties.Create(
            id: id,
            title: "Nice apartment",
            city: "Trondheim",
            price: 4_500_000m,
            type: PropertyType.Apartment,
            status: PropertyStatus.Active,
            createdAt: DateTime.UtcNow,
            brokerId: brokerId
        );

        await repo.CreateAsync(entity, CancellationToken.None);

        var found = await repo.GetByIdAsync(id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(id);
        found.City.Should().Be("Trondheim");
        found.Type.Should().Be(PropertyType.Apartment);
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated_entity()
    {
        var repo = CreateRepo();

        var entity = TestProperties.Create(
            title: "Old title",
            city: "Oslo",
            price: 12_000_000m,
            type: PropertyType.House,
            status: PropertyStatus.Active,
            createdAt: DateTime.UtcNow,
            brokerId: Guid.NewGuid()
        );

        await repo.CreateAsync(entity, CancellationToken.None);

        entity.Title = "New title";
        var updated = await repo.UpdateAsync(entity, CancellationToken.None);
        updated.Should().BeTrue();

        var found = await repo.GetByIdAsync(entity.Id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.Title.Should().Be("New title");
    }

    [Fact]
    public async Task GetList_filters_by_city_and_type()
    {
        var repo = CreateRepo();

        var brokerId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await repo.CreateAsync(TestProperties.Create(
            title: "A",
            city: "Trondheim",
            price: 3_000_000m,
            type: PropertyType.Apartment,
            status: PropertyStatus.Active,
            createdAt: now.AddMinutes(-1),
            brokerId: brokerId
        ), CancellationToken.None);

        await repo.CreateAsync(TestProperties.Create(
            title: "B",
            city: "Trondheim",
            price: 6_000_000m,
            type: PropertyType.House,
            status: PropertyStatus.Active,
            createdAt: now.AddMinutes(-2),
            brokerId: brokerId
        ), CancellationToken.None);

        var query = new PropertyListQuery(
            City: "Trondheim",
            Type: "Apartment",
            Status: null,
            MinPrice: null,
            MaxPrice: null,
            BrokerId: null,
            Page: 1,
            PageSize: 10,
            Sort: "createdAtDesc"
        );

        var (items, total) = await repo.GetListAsync(query, CancellationToken.None);

        total.Should().Be(1);
        items.Should().HaveCount(1);
        items[0].Type.Should().Be(PropertyType.Apartment);
    }
}
