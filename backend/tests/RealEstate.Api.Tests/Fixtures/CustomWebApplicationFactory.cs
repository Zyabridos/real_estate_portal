using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace RealEstate.Api.Tests.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IMongoDatabase _db;

    public CustomWebApplicationFactory(IMongoDatabase db)
    {
        _db = db;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing IMongoDatabase registrations
            var mongoDbDescriptors = services
                .Where(d => d.ServiceType == typeof(IMongoDatabase))
                .ToList();

            foreach (var d in mongoDbDescriptors)
                services.Remove(d);

            // Remove IMongoClient too (optional but clean)
            var mongoClientDescriptors = services
                .Where(d => d.ServiceType == typeof(IMongoClient))
                .ToList();

            foreach (var d in mongoClientDescriptors)
                services.Remove(d);

            // Replace with fixture database
            services.AddSingleton(_db);
            services.AddSingleton(_db.Client);
        });
    }
}