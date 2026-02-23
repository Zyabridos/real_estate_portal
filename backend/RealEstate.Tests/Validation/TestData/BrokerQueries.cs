using RealEstate.Application.Features.Brokers.List;

namespace RealEstate.Validation.Tests.TestData;

public static class BrokerQueries
{
    public static BrokerListQuery Valid() => new(Page: 1, PageSize: 20);
}