using RealEstate.Application.DTOs.Leads;
using RealEstate.Application.Validation.Leads;
using Xunit;

namespace RealEstate.Validation.Tests.Validation;

public sealed class CreateLeadRequestValidatorTests
{
    private readonly CreateLeadRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var dto = Valid();

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void PropertyId_is_required()
    {
        var dto = Valid() with { PropertyId = Guid.Empty };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.PropertyId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void FullName_must_have_min_length_2(string fullName)
    {
        var dto = Valid() with { FullName = fullName };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.FullName));
    }

    [Fact]
    public void FullName_max_length_50()
    {
        var dto = Valid() with { FullName = new string('a', 51) };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.FullName));
    }

    [Fact]
    public void Valid_when_email_present_and_phone_missing()
    {
        var dto = Valid() with { PhoneNumber = null };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Valid_when_phone_present_and_email_missing()
    {
        var dto = Valid() with { Email = null };

        var result = _validator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Invalid_when_email_and_phone_missing()
    {
        var dto = Valid() with { Email = null, PhoneNumber = null };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Email_must_be_valid_format(string email)
    {
        var dto = Valid() with { Email = email };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Email));
    }

    [Fact]
    public void Email_max_length_100()
    {
        var local = new string('a', 97);
        var dto = Valid() with { Email = $"{local}@a.com" }; // 102 chars

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Email));
    }

    [Fact]
    public void PhoneNumber_max_length_20()
    {
        var dto = Valid() with { PhoneNumber = new string('1', 21) };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.PhoneNumber));
    }

    [Fact]
    public void Message_max_length_2000()
    {
        var dto = Valid() with { Message = new string('a', 2001) };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Message));
    }

    private static CreateLeadRequest Valid() =>
        new(
            PropertyId: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FullName: "John Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+47 999 99 999",
            Message: "Hello World!"
        );
}
