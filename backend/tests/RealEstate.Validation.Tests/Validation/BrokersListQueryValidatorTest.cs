using FluentValidation.Results;
using RealEstate.Application.Queries.Brokers;
using RealEstate.Application.Validation.Brokers;
using Xunit;

namespace RealEstate.Validation.Tests.Validation;

public sealed class BrokersListQueryValidatorTest
{
    private readonly BrokerListQueryValidator _validator = new();

    [Fact]
    public void Valid_query_passes()
    {
        var query = Valid();

        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Page_must_be_at_least_1(int page)
    {
        var query = Valid() with { Page = page };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(BrokerListQuery.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSize_must_be_at_least_1(int pageSize)
    {
        var query = Valid() with { PageSize = pageSize };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(BrokerListQuery.PageSize));
    }

    [Fact]
    public void PageSize_must_not_exceed_100()
    {
        var query = Valid() with { PageSize = 101 };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(BrokerListQuery.PageSize));
    }

    [Fact]
    public void PageSize_100_is_allowed()
    {
        var query = Valid() with { PageSize = 100 };

        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    private static BrokerListQuery Valid() =>
        new(
            BrokerId: null,
            FirstName: null,
            LastName: null,
            AgencyId: null,
            Email: null,
            PhoneNumber: null,
            Page: 1,
            PageSize: 20,
            SortBy: null,
            SortDirection: null
        );

    private static void AssertHasErrorFor(ValidationResult result, string propertyName)
    {
        Assert.Contains(result.Errors, e => e.PropertyName == propertyName);
    }
}
