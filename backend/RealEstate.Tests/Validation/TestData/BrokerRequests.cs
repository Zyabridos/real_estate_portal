using RealEstate.Application.Features.Brokers.Create;
using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Validation.Tests.TestData;

public static class BrokerRequests
{
    public static CreateBrokerRequest Valid() =>
        new(
            FirstName: "John",
            LastName: "Snow",
            Email: "johnsnow@winterfell.com",
            PhoneNumber: "+4766666666",
            Gender: BrokerGender.Male
        );
}