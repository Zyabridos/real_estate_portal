using RealEstate.Application.DTOs.Leads;
using RealEstate.Application.Validation.Leads;
using RealEstate.Validation.Tests.Common;
using RealEstate.Validation.Tests.TestData;
using Xunit;

namespace RealEstate.Validation.Tests.Validators.Leads;

public sealed class CreateLeadRequestValidatorTests
{
    private readonly CreateLeadRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var dto = LeadRequests.Valid();

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldBeValid(result);
    }

    [Fact]
    public void PropertyId_is_required()
    {
        var dto = LeadRequests.Valid() with { PropertyId = Guid.Empty };

        var result = _validator.Validate(dto);

        ValidationAssertions.ShouldHaveErrorFor(result, nameof(CreateLeadRequest.PropertyId));
    }

    [Fact]
    public void Invalid_when_email_and_phone_missing()
    {
        var dto = LeadRequests.Valid() with { Email = null, PhoneNumber = null };

        var result = _validator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}