using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Features.Leads.GetById;
using RealEstate.Domain.Enums.Leads;

namespace RealEstate.Api.Swagger.Examples.Leads;

public sealed class LeadDetailsResponseExample : IExamplesProvider<LeadDetailsDto>
{
    public LeadDetailsDto GetExamples()
    {
        return new LeadDetailsDto(
            Id: 1,
            AgencyId: 1,
            BrokerId: 1002,
            PropertyId: 123,
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