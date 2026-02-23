using RealEstate.Application.Features.Properties.List;

namespace RealEstate.Validation.Tests.TestData;

public static class PropertyQueries
{
    public static PropertyListQuery Valid() =>
        new(
            City: null,
            Type: null,
            Status: null,
            BrokerId: null,
            MinPrice: null,
            MaxPrice: null,
            Page: 1,
            PageSize: 20
        );
}