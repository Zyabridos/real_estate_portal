using RealEstate.Application.Features.Brokers.Create;

namespace RealEstate.Validation.Tests.TestData;

public static class BrokerRequests
{
    public static CreateBrokerRequest Valid() =>
        new(
            FirstName: "John",
            LastName: "Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+4766666666"
        );
}