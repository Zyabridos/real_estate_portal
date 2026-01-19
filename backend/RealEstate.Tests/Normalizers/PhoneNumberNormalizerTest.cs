using RealEstate.Application.Common.Normalizers;
using Xunit;

namespace RealEstate.Validation.Tests.Common;

public sealed class PhoneNumberNormalizerTests
{
    [Theory]
    [InlineData("+47 123 12 123", "+4712312123")]
    [InlineData("+1 123 345 1234", "+11233451234")]
    [InlineData("+47 123-12-123", "+4712312123")]
    [InlineData("+4712312123", "+4712312123")]
    [InlineData("12312123", "+4712312123")]
    [InlineData("123-123-23", "+4712312323")]
    [InlineData("123 12 123", "+4712312123")]
    [InlineData("12 312 123", "+4712312123")]
    [InlineData("1 23 1 2123", "+4712312123")]
    public void TryNormalize_valid_inputs_returns_normalized(string input, string expected)
    {
        var ok = PhoneNumberNormalizer.TryNormalize(input, out var normalized, out var error);

        Assert.True(ok);
        Assert.Equal(expected, normalized);
        Assert.True(string.IsNullOrEmpty(error));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryNormalize_empty_inputs_fails(string? input)
    {
        var ok = PhoneNumberNormalizer.TryNormalize(input, out var normalized, out var error);

        Assert.False(ok);
        Assert.Equal(string.Empty, normalized);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }

    [Theory]
    // limit of separators (combined) is 3
    [InlineData("+47 12-3-12-123")]
    [InlineData("1 2 3 4 5 6 7 8")]
    public void TryNormalize_more_than_3_separators_fails(string input)
    {
        var ok = PhoneNumberNormalizer.TryNormalize(input, out _, out var error);

        Assert.False(ok);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }

    [Theory]
    [InlineData("47+12312123")]
    [InlineData("++4712312123")]
    [InlineData("+47+12312123")]
    public void TryNormalize_plus_not_only_at_start_fails(string input)
    {
        var ok = PhoneNumberNormalizer.TryNormalize(input, out _, out var error);

        Assert.False(ok);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }

    [Theory]
    [InlineData("+47 (123) 12 123")]
    [InlineData("+47 123.12.123")]
    [InlineData("+47 abc 12 123")]
    public void TryNormalize_invalid_characters_fails(string input)
    {
        var ok = PhoneNumberNormalizer.TryNormalize(input, out _, out var error);

        Assert.False(ok);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("+47123456789012345678901")] // 21 character
    public void TryNormalize_digits_out_of_range_fails(string input)
    {
        var ok = PhoneNumberNormalizer.TryNormalize(input, out _, out var error);

        Assert.False(ok);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }
}
