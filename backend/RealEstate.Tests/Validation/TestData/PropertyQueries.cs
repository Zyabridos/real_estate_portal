using RealEstate.Application.Features.Properties.List;

namespace RealEstate.Validation.Tests.TestData;

public static class PropertyQueries
{
    public static PropertyListQuery Valid() =>
        new(
            BrokerId: 123,
            AgencyId: 1,
            City: null,
            Type: null,
            Status: null,
            MinPrice: null,
            MaxPrice: null,
            Page: 1,
            PageSize: 20
        );
}