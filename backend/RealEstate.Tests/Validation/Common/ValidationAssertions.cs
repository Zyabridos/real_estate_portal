using FluentValidation.Results;
using Xunit;

namespace RealEstate.Validation.Tests.Common;

public static class ValidationAssertions
{
    public static void ShouldBeValid(ValidationResult result)
    {
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    public static void ShouldHaveErrorFor(ValidationResult result, string propertyName)
    {
        Assert.Contains(result.Errors, e => e.PropertyName == propertyName);
    }

    public static void ShouldNotHaveErrorFor(ValidationResult result, string propertyName)
    {
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == propertyName);
    }

    public static void ShouldHaveGlobalErrorWithMessage(ValidationResult result, string message)
    {
        Assert.Contains(result.Errors, e => e.PropertyName == string.Empty && e.ErrorMessage == message);
    }

    public static void ShouldNotHaveGlobalErrorWithMessage(ValidationResult result, string message)
    {
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == string.Empty && e.ErrorMessage == message);
    }
}