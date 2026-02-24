using RealEstate.Application.Features.Agencies.List;

namespace RealEstate.Validation.Tests.TestData;

public static class AgencyQueries
{
    public static AgencyListQuery Valid() => new(Page: 1, PageSize: 20);
}