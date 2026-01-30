using RealEstate.Application.Features.Brokers.List;
using RealEstate.Validation.Tests.Common;
using RealEstate.Validation.Tests.TestData;
using Xunit;

namespace RealEstate.Validation.Tests.Validators.Brokers;

public sealed class BrokerListQueryValidatorTests
{
    private readonly BrokerListQueryValidator _validator = new();

    [Fact]
    public void Valid_query_passes()
    {
        var query = BrokerQueries.Valid();

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldBeValid(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Page_must_be_at_least_1(int page)
    {
        var query = BrokerQueries.Valid() with { Page = page };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(BrokerListQuery.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSize_must_be_at_least_1(int pageSize)
    {
        var query = BrokerQueries.Valid() with { PageSize = pageSize };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(BrokerListQuery.PageSize));
    }

    [Fact]
    public void PageSize_must_not_exceed_100()
    {
        var query = BrokerQueries.Valid() with { PageSize = 101 };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(BrokerListQuery.PageSize));
    }

    [Fact]
    public void PageSize_100_is_allowed()
    {
        var query = BrokerQueries.Valid() with { PageSize = 100 };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldBeValid(result);
    }
}