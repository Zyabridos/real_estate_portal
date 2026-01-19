using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RealEstate.Api;

namespace RealEstate.TestData.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IMongoDatabase _db;
    private readonly string _connectionString;
    private readonly string _databaseName;

    public CustomWebApplicationFactory(
        IMongoDatabase db,
        string connectionString,
        string databaseName)
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
                ["Mongo:DatabaseName"] = _databaseName
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing IMongoDatabase registrations (если были)
            foreach (var d in services.Where(x => x.ServiceType == typeof(IMongoDatabase)).ToList())
                services.Remove(d);

            // Remove IMongoClient too
            foreach (var d in services.Where(x => x.ServiceType == typeof(IMongoClient)).ToList())
                services.Remove(d);

            // Replace with fixture database
            services.AddSingleton(_db);
            services.AddSingleton(_db.Client);
        });
    }
}