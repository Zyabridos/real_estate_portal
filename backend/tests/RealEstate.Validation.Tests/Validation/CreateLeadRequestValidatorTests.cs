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

    [Theory]
    [InlineData("")]

// ReSharper disable once xUnit1006
    [InlineData(null)]
    public void PropertyId_is_required(string? propertyId)
    {
        var dto = Valid() with { PropertyId = propertyId! };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.PropertyId));
    }

    [Fact]
    public void PropertyId_max_length_64()
    {
        var dto = Valid() with { PropertyId = new string('a', 65) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.PropertyId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Name_must_have_min_length_2(string name)
    {
        var dto = Valid() with { Name = name };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Name));
    }

    [Fact]
    public void Name_max_length_50()
    {
        var dto = Valid() with { Name = new string('a', 51) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Name));
    }

    [Theory]
    [InlineData("")]

// ReSharper disable once xUnit1006
    [InlineData(null)]
    public void Email_is_required(string? email)
    {
        var dto = Valid() with { Email = email! };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Email));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Email_must_be_valid_format(string email)
    {
        var dto = Valid() with { Email = email };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Email));
    }

    [Fact]
    public void Email_max_length_100()
    {
        var local = new string('a', 97);
        var dto = Valid() with { Email = $"{local}@a.com" }; // 102 chars

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Email));
    }

    [Fact]
    public void Phone_max_length_20()
    {
        var dto = Valid() with { Phone = new string('1', 21) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Phone));
    }

    [Fact]
    public void Message_max_length_2000()
    {
        var dto = Valid() with { Message = new string('a', 2001) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeadRequest.Message));
    }

    private static CreateLeadRequest Valid() =>
        new(
            PropertyId: "property_123",
            Name: "John Snow",
            Email: "johnsnow@winterfell.com",
            Phone: "+47 999 99 999",
            Message: "Hello World!"
        );
}
