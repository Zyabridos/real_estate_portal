using RealEstate.Application.Features.Leads.Create;

namespace RealEstate.Validation.Tests.TestData;

public static class LeadRequests
{
    public static CreateLeadRequest Valid() =>
        new(
            PropertyId: 1,
            FullName: "John Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+4766666666",
            Message: "Please contact me"
        );
}