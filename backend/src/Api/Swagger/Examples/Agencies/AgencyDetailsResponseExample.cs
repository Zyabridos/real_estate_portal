using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Features.Agencies.GetById;

namespace RealEstate.Api.Swagger.Examples.Agencies;

public sealed class AgencyDetailsResponseExample : IExamplesProvider<AgencyDetailsDto>
{
    public AgencyDetailsDto GetExamples()
    {
        return new AgencyDetailsDto(
            Id: Guid.Parse("11111111-aaaa-bbbb-1111-111111111111"),
            Name: "Eiendommenbyrå #1 i Bodø",
            OrgNumber: "0004567891011",
            PhoneNumber: "+47 234 11 895",
            City: "Bodø",
            Street: "Hovedgata 12",
            ZipCode: "5002",
            CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
            UpdatedAt: DateTime.Parse("2026-01-10T13:00:00Z")
        );
    }
}