using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealEstate.Api.Health;

namespace RealEstate.Api.DependencyInjection;

public static class ApiWebApplicationExtensions
{
    /// <summary>
    /// Composition root for HTTP pipeline + endpoints.
    /// Keep Program.cs tiny: app.UseApi()
    /// </summary>
    public static WebApplication UseApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        MapHealthEndpoints(app);

        return app;
    }

    private static void MapHealthEndpoints(WebApplication app)
    {
        app.MapHealthChecks("/api/health/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
            ResponseWriter = HealthResponseWriter.WriteLiveAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status200OK
            }
        });

        // Mongo ok ? 200 : 503
        app.MapHealthChecks("/api/health/ready", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("ready"),
            ResponseWriter = HealthResponseWriter.WriteReadyAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // keep it for now - some services still using it. TODO: move everything to /ready and /live, then delete code
        app.MapHealthChecks("/api/health", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("ready"),
            ResponseWriter = HealthResponseWriter.WriteReadyAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });
    }
}