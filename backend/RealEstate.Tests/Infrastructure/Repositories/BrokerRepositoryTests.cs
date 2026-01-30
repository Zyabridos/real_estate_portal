using Xunit;
using FluentAssertions;
using RealEstate.Infrastructure.Repositories.Brokers;

using RealEstate.Application.Features.Brokers.List;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Brokers;
using RealEstate.Domain.Enums.Common;

using RealEstate.Infrastructure.Repositories;

using RealEstate.TestData;
using RealEstate.TestData.Mongo;

namespace RealEstate.Infrastructure.Tests;

[Collection("MongoDb")]
public sealed class BrokerRepositoryTests : MongoDbTestBase
{
    public BrokerRepositoryTests(MongoDbFixture fixture) : base(fixture)
    {
    }

    private BrokerRepository CreateRepo() => new(Fixture.Database);

    [Fact]
    public async Task Create_then_GetById_returns_entity()
    {
        var repo = CreateRepo();

        var id = Guid.NewGuid();

        var entity = TestBrokers.Create(
            id: id,
            firstName: "Ola",
            lastName: "Nordmann",
            email: "ola.nordmann@realestate.no",
            phoneNumber: "+47 111 11 111",
            createdAt: DateTime.UtcNow
        );

        await repo.CreateAsync(entity, CancellationToken.None);

        var found = await repo.GetById(id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(id);
        found.FirstName.Should().Be("Ola");
        found.LastName.Should().Be("Nordmann");
        found.Email.Should().Be("ola.nordmann@realestate.no");

        // Normalization smoke (depends on repo behavior)
        found.PhoneNumber.Should().Be("+4711111111");
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated_entity()
    {
        var repo = CreateRepo();

        var entity = TestBrokers.Create(
            firstName: "Arya",
            lastName: "Stark",
            email: "arya.stark@realestate.no",
            phoneNumber: "+47 222 22 222",
            createdAt: DateTime.UtcNow
        );

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
        var repo = CreateRepo();

        var now = DateTime.UtcNow;

        var matching = TestBrokers.Create(
            firstName: "Ola",
            lastName: "Nordmann",
            email: "ola.nordmann@realestate.no",
            phoneNumber: "+47 111 11 111",
            createdAt: now.AddMinutes(-1)
        );

        var other1 = TestBrokers.Create(
            firstName: "Ola",
            lastName: "Hansen",
            email: "ola.hansen@realestate.no",
            phoneNumber: "+47 333 33 333",
            createdAt: now.AddMinutes(-2)
        );

        var other2 = TestBrokers.Create(
            firstName: "Kari",
            lastName: "Nordmann",
            email: "kari.nordmann@realestate.no",
            phoneNumber: "+47 444 44 444",
            createdAt: now.AddMinutes(-3)
        );

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
        var repo = CreateRepo();

        var now = DateTime.UtcNow;

        var matching = TestBrokers.Create(
            firstName: "Kari",
            lastName: "Hansen",
            email: "kari.hansen@realestate.no",
            phoneNumber: "+47 999 99 999",
            createdAt: now.AddMinutes(-1)
        );

        var other1 = TestBrokers.Create(
            firstName: "Kari",
            lastName: "Hansen",
            email: "kari.hansen2@realestate.no",
            phoneNumber: "+47 999 99 999",
            createdAt: now.AddMinutes(-2)
        );

        var other2 = TestBrokers.Create(
            firstName: "Kari",
            lastName: "Hansen",
            email: "kari.hansen@realestate.no",
            phoneNumber: "+47 888 88 888",
            createdAt: now.AddMinutes(-3)
        );

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
        items[0].PhoneNumber.Should().Be("+4799999999");
    }
}
