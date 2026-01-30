using RealEstate.Application.Features.Brokers.List;

namespace RealEstate.Validation.Tests.TestData;

public static class BrokerQueries
{
    public static BrokerListQuery Valid() =>
        new(
            BrokerId: null,
            FirstName: null,
            LastName: null,
            AgencyId: null,
            Email: null,
            PhoneNumber: null,
            Page: 1,
            PageSize: 20,
            SortBy: null,
            SortDirection: null
        );
}