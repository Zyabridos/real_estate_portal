using RealEstate.Application.Features.Agencies.List;
using RealEstate.Validation.Tests.Common;
using RealEstate.Validation.Tests.TestData;
using Xunit;

namespace RealEstate.Validation.Tests.Validators.Agencies;

public sealed class AgencyListQueryValidatorTests
{
    private readonly AgencyListQueryValidator _validator = new();

    [Fact]
    public void Valid_query_passes()
    {
        var query = AgencyQueries.Valid();

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldBeValid(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Page_must_be_at_least_1(int page)
    {
        var query = AgencyQueries.Valid() with { Page = page };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(AgencyListQuery.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSize_must_be_at_least_1(int pageSize)
    {
        var query = AgencyQueries.Valid() with { PageSize = pageSize };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(AgencyListQuery.PageSize));
    }

    [Fact]
    public void PageSize_must_not_exceed_100()
    {
        var query = AgencyQueries.Valid() with { PageSize = 101 };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(AgencyListQuery.PageSize));
    }

    [Fact]
    public void PageSize_100_is_allowed()
    {
        var query = AgencyQueries.Valid() with { PageSize = 100 };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldBeValid(result);
    }
}