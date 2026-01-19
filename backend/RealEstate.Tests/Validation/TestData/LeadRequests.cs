using RealEstate.Application.DTOs.Leads;

namespace RealEstate.Validation.Tests.TestData;

public static class LeadRequests
{
    public static CreateLeadRequest Valid(Guid? propertyId = null) =>
        new(
            PropertyId: propertyId ?? Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FullName: "John Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+47 999 99 999",
            Message: "Hello World!"
        );
}