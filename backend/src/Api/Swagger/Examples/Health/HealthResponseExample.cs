using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Features.Health;

namespace RealEstate.Api.Swagger.Examples.Health;

public sealed class HealthResponseExample : IExamplesProvider<HealthDto>
{
    private readonly IWebHostEnvironment _env;

    public HealthResponseExample(IWebHostEnvironment env)
    {
        _env = env;
    }

    public HealthDto GetExamples() =>
        new(
            Status: "ok",
            Service: "realestate-api",
            Environment: _env.EnvironmentName,
            Mongo: "ok"
        );
}