using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealEstate.Application.Features.Health;

namespace RealEstate.Api.Health;

public static class HealthResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Task WriteLiveAsync(HttpContext context, HealthReport report) =>
        WriteAsync(context, report, mongoFallback: "skipped");

    public static Task WriteReadyAsync(HttpContext context, HealthReport report) =>
        WriteAsync(context, report, mongoFallback: "unknown");

    private static async Task WriteAsync(HttpContext context, HealthReport report, string mongoFallback)
    {
        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();

        var overall = ToStatusString(report.Status);

        var mongo = report.Entries.TryGetValue("mongo", out var mongoEntry)
            ? ToStatusString(mongoEntry.Status)
            : mongoFallback;

        var dto = new HealthDto(
            Status: overall,
            Service: "realestate-api",
            Environment: env.EnvironmentName,
            Mongo: mongo);

        context.Response.ContentType = "application/json; charset=utf-8";
        await JsonSerializer.SerializeAsync(context.Response.Body, dto, JsonOptions, context.RequestAborted);
    }

    private static string ToStatusString(HealthStatus status) =>
        status switch
        {
            HealthStatus.Healthy => "ok",
            HealthStatus.Degraded => "degraded",
            HealthStatus.Unhealthy => "unhealthy",
            _ => "unknown"
        };
}