using RealEstate.Application.DTOs.Brokers;
using RealEstate.Application.Validation.Brokers;
using Xunit;

namespace RealEstate.Validation.Tests.Validation;

public sealed class CreateBrokerRequestValidatorTests
{
    private readonly CreateBrokerRequestValidator _validator = new();

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
    [InlineData(" ")]
    [InlineData("A")]
    public void FirstName_must_have_min_length_2(string firstName)
    {
        var dto = Valid() with { FirstName = firstName };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.FirstName));
    }

    [Fact]
    public void FirstName_max_length_50()
    {
        var dto = Valid() with { FirstName = new string('a', 51) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void LastName_must_have_min_length_2(string lastName)
    {
        var dto = Valid() with { LastName = lastName };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.LastName));
    }

    [Fact]
    public void LastName_max_length_50()
    {
        var dto = Valid() with { LastName = new string('a', 51) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.LastName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FirstName_is_required(string? firstName)
    {
        var dto = Valid() with { FirstName = firstName };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.FirstName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LastName_is_required(string? lastName)
    {
        var dto = Valid() with { LastName = lastName };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.LastName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void PhoneNumber_is_required(string? phoneNumber)
    {
        var dto = Valid() with { PhoneNumber = phoneNumber };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.PhoneNumber));
    }

    [Fact]
    public void PhoneNumber_max_length_20()
    {
        var dto = Valid() with { PhoneNumber = new string('1', 21) };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.PhoneNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Email_is_required(string? email)
    {
        var dto = Valid() with { Email = email };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.Email));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Email_must_be_valid_format(string email)
    {
        var dto = Valid() with { Email = email };

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.Email));
    }

    [Fact]
    public void Email_max_length_100()
    {
        var local = new string('a', 97);
        var dto = Valid() with { Email = $"{local}@a.com" }; // 103 chars (97 + 6)

        var result = _validator.Validate(dto);

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBrokerRequest.Email));
    }

    private static CreateBrokerRequest Valid() =>
        new(
            AgencyId: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName: "John",
            LastName: "Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+4766666666"
        );
}
