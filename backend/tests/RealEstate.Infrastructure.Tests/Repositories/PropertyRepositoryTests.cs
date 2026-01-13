using FluentAssertions;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Infrastructure.Tests.Fixtures;
using Xunit;

namespace RealEstate.Infrastructure.Tests.Repositories;

[Collection("MongoDb")]
public sealed class PropertyRepositoryTests : MongoDbTestBase
{
    public PropertyRepositoryTests(MongoDbFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Create_then_GetById_returns_entity()
    {
        var repo = new PropertyRepository(Fixture.Database);

        var id = Guid.NewGuid();
        var brokerId = Guid.NewGuid();

        var entity = new Property
        {
            Id = id,
            Title = "Nice apartment",
            Address = "Street 1",
            City = "Trondheim",
            Price = 4_500_000m,
            Type = PropertyType.Apartment,
            Bedrooms = 2,
            Bathrooms = 1,
            Area = 66.6,
            BrokerId = brokerId,
            Status = PropertyStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await repo.CreateAsync(entity, CancellationToken.None);

        var found = await repo.FindByIdAsync(id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(id);
        found.City.Should().Be("Trondheim");
        found.Type.Should().Be(PropertyType.Apartment);
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated_entity()
    {
        var repo = new PropertyRepository(Fixture.Database);

        var entity = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Old title",
            Address = "Street 1",
            City = "Oslo",
            Price = 12_000_000m,
            Type = PropertyType.House,
            Bedrooms = 3,
            Bathrooms = 2,
            Area = 120,
            BrokerId = Guid.NewGuid(),
            Status = PropertyStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await repo.CreateAsync(entity, CancellationToken.None);

        entity.Title = "New title";
        var updated = await repo.UpdateAsync(entity, CancellationToken.None);
        updated.Should().BeTrue();

        var found = await repo.FindByIdAsync(entity.Id, CancellationToken.None);
        found!.Title.Should().Be("New title");
    }

    [Fact]
    public async Task FindPaged_filters_by_city_and_type()
    {
        var repo = new PropertyRepository(Fixture.Database);

        var broker = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await repo.CreateAsync(new Property
        {
            Id = Guid.NewGuid(),
            Title = "A",
            Address = "1",
            City = "Trondheim",
            Price = 3_000_000m,
            Type = PropertyType.Apartment,
            Bedrooms = 1,
            Bathrooms = 1,
            Area = 40,
            BrokerId = broker,
            Status = PropertyStatus.Active,
            CreatedAt = now.AddMinutes(-1)
        }, CancellationToken.None);

        await repo.CreateAsync(new Property
        {
            Id = Guid.NewGuid(),
            Title = "B",
            Address = "2",
            City = "Trondheim",
            Price = 6_000_000m,
            Type = PropertyType.House,
            Bedrooms = 3,
            Bathrooms = 2,
            Area = 120,
            BrokerId = broker,
            Status = PropertyStatus.Active,
            CreatedAt = now.AddMinutes(-2)
        }, CancellationToken.None);

        var page = await repo.FindPagedAsync(
            city: "Trondheim",
            type: PropertyType.Apartment,
            status: null,
            minPrice: null,
            maxPrice: null,
            page: 1,
            pageSize: 10,
            ct: CancellationToken.None);

        page.TotalCount.Should().Be(1);
        page.Items.Should().HaveCount(1);
        page.Items[0].Type.Should().Be(PropertyType.Apartment);
    }
}
