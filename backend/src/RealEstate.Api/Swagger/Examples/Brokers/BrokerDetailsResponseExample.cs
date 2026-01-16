using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.DTOs.Brokers;

namespace RealEstate.Api.Swagger.Examples.Brokers;

public sealed class BrokerDetailsResponseExample : IExamplesProvider<BrokerDetailsDto>
{
    public BrokerDetailsDto GetExamples()
    {
        return new BrokerDetailsDto(
            Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            AgencyId: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            FirstName: "Ola",
            LastName: "Nordmann",
            Email: "ola.nordmann@realestate.no",
            PhoneNumber: "+47 111 11 111",
            PhotoUrl: "https://cdn.example.com/brokers/ola.jpg",
            CreatedAt: DateTime.Parse("2026-01-10T12:00:00Z"),
            UpdatedAt: DateTime.Parse("2026-01-11T10:30:00Z")
        );
    }
}