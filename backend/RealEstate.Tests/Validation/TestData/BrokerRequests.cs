using RealEstate.Application.Features.Brokers.Create;

namespace RealEstate.Validation.Tests.TestData;

public static class BrokerRequests
{
    public static CreateBrokerRequest Valid() =>
        new(
            AgencyId: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName: "John",
            LastName: "Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+4766666666"
        );
}