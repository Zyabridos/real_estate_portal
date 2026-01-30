using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Api.Swagger.Examples.Leads;

public sealed class LeadDetailsResponseExample : IExamplesProvider<LeadDetailsDto>
{
    public LeadDetailsDto GetExamples()
    {
        return new LeadDetailsDto(
            Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            PropertyId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            FullName: "Bran Stark",
            Email: "bran.stark@winterfale.com",
            PhoneNumber: "+47 111 22 333",
            Message: "I am interested in the property.",
            Status: LeadStatus.New,
            CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
            UpdatedAt: DateTime.Parse("2026-01-11T13:00:00Z")
        );
    }
}