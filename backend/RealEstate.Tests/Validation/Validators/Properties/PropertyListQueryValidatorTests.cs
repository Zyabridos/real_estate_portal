using FluentValidation.Results;
using RealEstate.Application.Queries.Properties;
using RealEstate.Application.Validation.Properties;
using Xunit;

namespace RealEstate.Validation.Tests.Validation;

public sealed class PropertyListQueryValidatorTests
{
    private readonly PropertyListQueryValidator _validator = new();

    [Fact]
    public void Valid_query_passes()
    {
        var query = Valid() with
        {
            Page = 1,
            PageSize = 20,
            MinPrice = 0,
            MaxPrice = 1_000_000
        };

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

        AssertHasErrorFor(result, nameof(PropertyListQuery.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSize_must_be_at_least_1(int pageSize)
    {
        var query = Valid() with { PageSize = pageSize };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(PropertyListQuery.PageSize));
    }

    [Fact]
    public void PageSize_must_not_exceed_100()
    {
        var query = Valid() with { PageSize = 101 };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(PropertyListQuery.PageSize));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void MinPrice_if_present_must_be_non_negative(decimal minPrice)
    {
        var query = Valid() with { MinPrice = minPrice };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(PropertyListQuery.MinPrice));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void MaxPrice_if_present_must_be_non_negative(decimal maxPrice)
    {
        var query = Valid() with { MaxPrice = maxPrice };

        var result = _validator.Validate(query);

        AssertHasErrorFor(result, nameof(PropertyListQuery.MaxPrice));
    }

    [Fact]
    public void MinPrice_must_be_less_or_equal_to_MaxPrice_when_both_present()
    {
        var query = Valid() with { MinPrice = 200, MaxPrice = 100 };

        var result = _validator.Validate(query);

        Assert.Contains(result.Errors, e =>
            e.PropertyName == string.Empty &&
            e.ErrorMessage == "minPrice must be less than or equal to maxPrice.");
    }

    [Fact]
    public void Price_range_rule_is_not_triggered_if_one_side_missing()
    {
        var q1 = Valid() with { MinPrice = 200, MaxPrice = null };
        var q2 = Valid() with { MinPrice = null, MaxPrice = 100 };

        var r1 = _validator.Validate(q1);
        var r2 = _validator.Validate(q2);

        Assert.DoesNotContain(r1.Errors, e => e.ErrorMessage == "minPrice must be less than or equal to maxPrice.");
        Assert.DoesNotContain(r2.Errors, e => e.ErrorMessage == "minPrice must be less than or equal to maxPrice.");
    }

    private static PropertyListQuery Valid() =>
        new(
            City: null,
            Type: null,
            Status: null,
            BrokerId: null,
            MinPrice: null,
            MaxPrice: null,
            Page: 1,
            PageSize: 20,
            Sort: null
        );

    private static void AssertHasErrorFor(ValidationResult result, string propertyName)
    {
        Assert.Contains(result.Errors, e => e.PropertyName == propertyName);
    }
}
