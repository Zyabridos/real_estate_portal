using RealEstate.Application.Features.Leads.List;

namespace RealEstate.Validation.Tests.TestData;

public static class LeadQueries
{
    public static LeadListQuery Valid() => new(Page: 1, PageSize: 20);
}