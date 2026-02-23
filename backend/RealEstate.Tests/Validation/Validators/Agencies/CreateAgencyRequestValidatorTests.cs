using RealEstate.Application.Features.Agencies.Create;
using RealEstate.Validation.Tests.Common;
using RealEstate.Validation.Tests.TestData;
using Xunit;

namespace RealEstate.Validation.Tests.Validators.Agencies;

public sealed class CreateAgencyRequestValidatorTests
{
    private readonly CreateAgencyRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var dto = AgencyRequests.Valid();

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldBeValid(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_is_required(string? name)
    {
        var dto = AgencyRequests.Valid() with { Name = name };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateAgencyRequest.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void Name_must_have_min_length_2(string name)
    {
        var dto = AgencyRequests.Valid() with { Name = name };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateAgencyRequest.Name));
    }

    [Fact]
    public void Name_max_length_50()
    {
        var dto = AgencyRequests.Valid() with { Name = new string('a', 51) };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateAgencyRequest.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OrgNumver_is_required(string? orgNumber)
    {
        var dto = AgencyRequests.Valid() with { OrgNumber = orgNumber };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateAgencyRequest.OrgNumber));
    }
    
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")]
    public void OrgNumber_must_have_min_length_6(string orgNumber)
    {
        var dto = AgencyRequests.Valid() with { OrgNumber = orgNumber };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateAgencyRequest.OrgNumber));
    }

    [Fact]
    public void OrgNumber_max_length_50()
    {
        var dto = AgencyRequests.Valid() with { OrgNumber = new string('a', 51) };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateAgencyRequest.OrgNumber));
    }
}
