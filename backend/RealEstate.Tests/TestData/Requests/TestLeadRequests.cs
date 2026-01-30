using RealEstate.Application.Features.Leads.Create;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Application.Features.Leads.List;
using RealEstate.Application.Features.Leads.Update;
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