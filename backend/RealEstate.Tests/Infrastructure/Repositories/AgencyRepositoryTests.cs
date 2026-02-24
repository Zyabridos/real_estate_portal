using FluentAssertions;
using RealEstate.Application.Features.Agencies.List;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Agencies;
using RealEstate.Domain.Enums.Common;
using RealEstate.Infrastructure.Repositories.Agencies;
using RealEstate.TestData.Mongo;
using Xunit;

namespace RealEstate.Infrastructure.Tests;

[Collection("MongoDb")]
public sealed class AgencyRepositoryTests : MongoDbTestBase
{
    public AgencyRepositoryTests(MongoDbFixture fixture) : base(fixture)
    {
    }

    private AgencyRepository CreateRepo() => new(Fixture.Database);

    [Fact]
    public async Task Create_then_GetById_returns_entity()
    {
        var repo = CreateRepo();

        var id = Guid.NewGuid();

        var entity = new Agency
        {
            Id = id,
            Name = "Test Agency",
            OrgNumber = "123123123",
            PhoneNumber = " +47 111 22 333 ",
            City = "Trondheim",
            Street = "Testgata 1",
            ZipCode = "7010",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repo.CreateAsync(entity, CancellationToken.None);

        var found = await repo.GetById(id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(id);
        found.Name.Should().Be("Test Agency");
        found.OrgNumber.Should().Be("123123123");
        
        found.PhoneNumber.Should().Be("+4711122333");
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated_entity()
    {
        var repo = CreateRepo();

        var entity = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            OrgNumber = "999999",
            PhoneNumber = "+47 999 99 999",
            City = "OldCity",
            Street = "OldStreet",
            ZipCode = "0000",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        await repo.CreateAsync(entity, CancellationToken.None);

        entity.Name = "New Name";
        entity.PhoneNumber = " +47 111 22 333 ";

        var updated = await repo.UpdateAsync(entity, CancellationToken.None);
        updated.Should().BeTrue();

        var found = await repo.GetById(entity.Id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.Name.Should().Be("New Name");

        // см. комментарий выше
        found.PhoneNumber.Should().Be("+4711122333");
    }

    [Fact]
    public async Task GetList_filters_by_name_orgNumber_city()
    {
        var repo = CreateRepo();
        var now = DateTime.UtcNow;

        var matching = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "Alpha",
            OrgNumber = "111111",
            PhoneNumber = "+47 111 11 111",
            City = "Trondheim",
            Street = "A 1",
            ZipCode = "7010",
            CreatedAt = now.AddMinutes(-1),
            UpdatedAt = now.AddMinutes(-1)
        };

        var other1 = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "Alpha",
            OrgNumber = "222222",
            PhoneNumber = "+47 222 22 222",
            City = "Trondheim",
            Street = "B 1",
            ZipCode = "7010",
            CreatedAt = now.AddMinutes(-2),
            UpdatedAt = now.AddMinutes(-2)
        };

        var other2 = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "Beta",
            OrgNumber = "111111",
            PhoneNumber = "+47 333 33 333",
            City = "Oslo",
            Street = "C 1",
            ZipCode = "0001",
            CreatedAt = now.AddMinutes(-3),
            UpdatedAt = now.AddMinutes(-3)
        };

        await repo.CreateAsync(matching, CancellationToken.None);
        await repo.CreateAsync(other1, CancellationToken.None);
        await repo.CreateAsync(other2, CancellationToken.None);

        var query = new AgencyListQuery(
            Name: "Alpha",
            OrgNumber: "111111",
            City: "Trondheim",
            Page: 1,
            PageSize: 20,
            SortBy: SortBy.CreatedAt,
            SortDirection: SortDirection.Desc
        );

        var (items, total) = await repo.GetListAsync(query, CancellationToken.None);

        total.Should().Be(1);
        items.Should().HaveCount(1);
        items[0].Id.Should().Be(matching.Id);
        items[0].Name.Should().Be("Alpha");
        items[0].OrgNumber.Should().Be("111111");
        items[0].City.Should().Be("Trondheim");
    }

    [Fact]
    public async Task GetList_sorts_by_createdAt_desc()
    {
        var repo = CreateRepo();
        var now = DateTime.UtcNow;

        var a = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "A",
            OrgNumber = "1",
            PhoneNumber = "+47 1",
            City = "X",
            Street = "S",
            ZipCode = "Z",
            CreatedAt = now.AddMinutes(-10),
            UpdatedAt = now.AddMinutes(-10)
        };

        var b = new Agency
        {
            Id = Guid.NewGuid(),
            Name = "B",
            OrgNumber = "2",
            PhoneNumber = "+47 2",
            City = "X",
            Street = "S",
            ZipCode = "Z",
            CreatedAt = now.AddMinutes(-1),
            UpdatedAt = now.AddMinutes(-1)
        };

        await repo.CreateAsync(a, CancellationToken.None);
        await repo.CreateAsync(b, CancellationToken.None);

        var query = new AgencyListQuery(
            Page: 1,
            PageSize: 20,
            SortBy: SortBy.CreatedAt,
            SortDirection: SortDirection.Desc
        );

        var (items, total) = await repo.GetListAsync(query, CancellationToken.None);

        total.Should().Be(2);
        items.Should().HaveCount(2);
        items[0].Id.Should().Be(b.Id);
        items[1].Id.Should().Be(a.Id);
    }
}