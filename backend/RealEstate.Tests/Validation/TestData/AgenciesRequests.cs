using RealEstate.Application.Features.Agencies.Create;

namespace RealEstate.Validation.Tests.TestData;

public static class AgencyRequests
{
    public static CreateAgencyRequest Valid() =>
        new(
            Name: "Test Agency",
            OrgNumber: "123123123",
            PhoneNumber: "+47 111 22 333",
            City: "Trondheim",
            Street: "Testgata 1",
            ZipCode: "7010"
        );
}