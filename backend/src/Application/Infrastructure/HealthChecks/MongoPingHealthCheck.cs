using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace RealEstate.Infrastructure.HealthChecks;

public sealed class MongoPingHealthCheck : IHealthCheck
{
    private readonly IMongoDatabase _db;

    public MongoPingHealthCheck(IMongoDatabase db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1),
                cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB ping succeeded.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB ping failed.", ex);
        }
    }
}