using RealEstate.Application.DTOs.Leads;

namespace RealEstate.TestData.Requests;

public static class TestLeadRequests
{
    public static CreateLeadRequest Valid(Guid propertyId) =>
        new(
            PropertyId: propertyId,
            FullName: "John Snow",
            Email: "john@example.com",
            PhoneNumber: "+47 999 99 999",
            Message: "Hello!"
        );
}