using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Features.Brokers.GetById;
using RealEstate.Domain.Enums.Brokers;

namespace RealEstate.Api.Swagger.Examples.Brokers;

public sealed class BrokerDetailsResponseExample : IExamplesProvider<BrokerDetailsDto>
{
    public BrokerDetailsDto GetExamples()
    {
        return new BrokerDetailsDto(
            Id: 1000,
            AgencyId: 1,
            FirstName: "Ola",
            LastName: "Nordmann",
            Email: "ola.nordmann@realestate.no",
            PhoneNumber: "+47 111 11 111",
            Gender: BrokerGender.Female,
            PhotoUrl: "https://cdn.example.com/brokers/ola.jpg",
            CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
            UpdatedAt: DateTime.Parse("2026-01-11T10:30:00Z")
        );
    }
}