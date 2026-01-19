using RealEstate.Application.DTOs.Brokers;
using RealEstate.Application.Validation.Brokers;
using RealEstate.Validation.Tests.Common;
using RealEstate.Validation.Tests.TestData;
using Xunit;

namespace RealEstate.Validation.Tests.Validators.Brokers;

public sealed class CreateBrokerRequestValidatorTests
{
    private readonly CreateBrokerRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var dto = BrokerRequests.Valid();

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldBeValid(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FirstName_is_required(string? firstName)
    {
        var dto = BrokerRequests.Valid() with { FirstName = firstName };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateBrokerRequest.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void FirstName_must_have_min_length_2(string firstName)
    {
        var dto = BrokerRequests.Valid() with { FirstName = firstName };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateBrokerRequest.FirstName));
    }

    [Fact]
    public void FirstName_max_length_50()
    {
        var dto = BrokerRequests.Valid() with { FirstName = new string('a', 51) };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateBrokerRequest.FirstName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LastName_is_required(string? lastName)
    {
        var dto = BrokerRequests.Valid() with { LastName = lastName };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateBrokerRequest.LastName));
    }

    [Fact]
    public void PhoneNumber_is_required()
    {
        var dto = BrokerRequests.Valid() with { PhoneNumber = null };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateBrokerRequest.PhoneNumber));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Email_must_be_valid_format(string email)
    {
        var dto = BrokerRequests.Valid() with { Email = email };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateBrokerRequest.Email));
    }
}
