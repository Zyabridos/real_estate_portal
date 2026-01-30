using RealEstate.Application.Features.Properties.List;
using RealEstate.Validation.Tests.Common;
using RealEstate.Validation.Tests.TestData;
using Xunit;

namespace RealEstate.Validation.Tests.Validators.Properties;

public sealed class PropertyListQueryValidatorTests
{
    private readonly PropertyListQueryValidator _validator = new();

    [Fact]
    public void Valid_query_passes()
    {
        var query = PropertyQueries.Valid() with
        {
            MinPrice = 0,
            MaxPrice = 1_000_000
        };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldBeValid(result);
    }

    [Fact]
    public void MinPrice_must_be_less_or_equal_to_MaxPrice_when_both_present()
    {
        var query = PropertyQueries.Valid() with { MinPrice = 200, MaxPrice = 100 };

        var result = _validator.Validate(query);

        ValidationAssertions.ShouldHaveGlobalErrorWithMessage(result, "minPrice must be less than or equal to maxPrice.");
    }

    [Fact]
    public void Price_range_rule_is_not_triggered_if_one_side_missing()
    {
        var r1 = _validator.Validate(PropertyQueries.Valid() with { MinPrice = 200, MaxPrice = null });
        var r2 = _validator.Validate(PropertyQueries.Valid() with { MinPrice = null, MaxPrice = 100 });

        ValidationAssertions.ShouldNotHaveGlobalErrorWithMessage(r1, "minPrice must be less than or equal to maxPrice.");
        ValidationAssertions.ShouldNotHaveGlobalErrorWithMessage(r2, "minPrice must be less than or equal to maxPrice.");
    }
}