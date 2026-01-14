using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace RealEstate.Testing.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IMongoDatabase _db;
    private readonly string _connectionString;
    private readonly string _databaseName;

    public CustomWebApplicationFactory(IMongoDatabase db, string connectionString, string databaseName)
    {
        _db = db;
        _connectionString = connectionString;
        _databaseName = databaseName;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Mongo:ConnectionString"] = _connectionString,
                ["Mongo:Database"] = _databaseName
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing IMongoDatabase registrations
            var mongoDbDescriptors = services
                .Where(d => d.ServiceType == typeof(IMongoDatabase))
                .ToList();

            foreach (var d in mongoDbDescriptors)
                services.Remove(d);

            // Remove IMongoClient too
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