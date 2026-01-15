using FluentAssertions;
using RealEstate.Application.Queries.Brokers;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Brokers;
using RealEstate.Domain.Enums.Common;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Testing.Mongo;
using Xunit;

namespace RealEstate.Infrastructure.Tests.Repositories;

[Collection("MongoDb")]
public sealed class BrokerRepositoryTests : MongoDbTestBase
{
    public BrokerRepositoryTests(MongoDbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Create_then_GetById_returns_entity()
    {
        var repo = new BrokerRepository(Fixture.Database);

        var id = Guid.NewGuid();

        var entity = new Broker
        {
            Id = id,
            FirstName = "Ola",
            LastName = "Nordmann",
            Email = "ola.nordmann@realestate.no",
            PhoneNumber = "+47 111 11 111",
            CreatedAt = DateTime.UtcNow
        };

        await repo.CreateAsync(entity, CancellationToken.None);

        var found = await repo.GetById(id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(id);
        found.FirstName.Should().Be("Ola");
        found.LastName.Should().Be("Nordmann");
        found.Email.Should().Be("ola.nordmann@realestate.no");
        found.PhoneNumber.Should().Be("+47 111 11 111");
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated_entity()
    {
        var repo = new BrokerRepository(Fixture.Database);

        var entity = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Arya",
            LastName = "Stark",
            Email = "arya.stark@realestate.no",
            PhoneNumber = "+47 222 22 222",
            CreatedAt = DateTime.UtcNow
        };

        await repo.CreateAsync(entity, CancellationToken.None);

        entity.FirstName = "Sansa";

        var updated = await repo.UpdateAsync(entity, CancellationToken.None);
        updated.Should().BeTrue();

        var found = await repo.GetById(entity.Id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.FirstName.Should().Be("Sansa");
        found.LastName.Should().Be("Stark");
    }

    [Fact]
    public async Task GetList_filters_by_firstName_lastName()
    {
        var repo = new BrokerRepository(Fixture.Database);

        var matching = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Ola",
            LastName = "Nordmann",
            Email = "ola.nordmann@realestate.no",
            PhoneNumber = "+47 111 11 111",
            CreatedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        var other1 = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Ola",
            LastName = "Hansen",
            Email = "ola.hansen@realestate.no",
            PhoneNumber = "+47 333 33 333",
            CreatedAt = DateTime.UtcNow.AddMinutes(-2)
        };

        var other2 = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Kari",
            LastName = "Nordmann",
            Email = "kari.nordmann@realestate.no",
            PhoneNumber = "+47 444 44 444",
            CreatedAt = DateTime.UtcNow.AddMinutes(-3)
        };

        await repo.CreateAsync(matching, CancellationToken.None);
        await repo.CreateAsync(other1, CancellationToken.None);
        await repo.CreateAsync(other2, CancellationToken.None);

        var query = new BrokerListQuery(
            FirstName: "Ola",
            LastName: "Nordmann",
            Page: 1,
            PageSize: 20,
            SortBy: SortBy.CreatedAt,
            SortDirection: SortDirection.Desc
        );

        var (items, total) = await repo.GetListAsync(query, CancellationToken.None);

        total.Should().Be(1);
        items.Should().HaveCount(1);
        items[0].Id.Should().Be(matching.Id);
        items[0].FirstName.Should().Be("Ola");
        items[0].LastName.Should().Be("Nordmann");
    }

    [Fact]
    public async Task GetList_filters_by_email_phoneNumber()
    {
        var repo = new BrokerRepository(Fixture.Database);

        var matching = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Kari",
            LastName = "Hansen",
            Email = "kari.hansen@realestate.no",
            PhoneNumber = "+47 999 99 999",
            CreatedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        var other1 = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Kari",
            LastName = "Hansen",
            Email = "kari.hansen2@realestate.no",
            PhoneNumber = "+47 999 99 999",
            CreatedAt = DateTime.UtcNow.AddMinutes(-2)
        };

        var other2 = new Broker
        {
            Id = Guid.NewGuid(),
            FirstName = "Kari",
            LastName = "Hansen",
            Email = "kari.hansen@realestate.no",
            PhoneNumber = "+47 888 88 888",
            CreatedAt = DateTime.UtcNow.AddMinutes(-3)
        };

        await repo.CreateAsync(matching, CancellationToken.None);
        await repo.CreateAsync(other1, CancellationToken.None);
        await repo.CreateAsync(other2, CancellationToken.None);

        var query = new BrokerListQuery(
            Email: "kari.hansen@realestate.no",
            PhoneNumber: "+47 999 99 999",
            Page: 1,
            PageSize: 20,
            SortBy: SortBy.CreatedAt,
            SortDirection: SortDirection.Desc
        );

        var (items, total) = await repo.GetListAsync(query, CancellationToken.None);

        total.Should().Be(1);
        items.Should().HaveCount(1);
        items[0].Id.Should().Be(matching.Id);
        items[0].Email.Should().Be("kari.hansen@realestate.no");
        items[0].PhoneNumber.Should().Be("+47 999 99 999");
    }
}
