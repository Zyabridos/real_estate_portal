using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Api.Health;
using RealEstate.Application.Features.Brokers.Contracts;
using RealEstate.Application.Features.Brokers.Services;
using RealEstate.Infrastructure.Mongo;
using RealEstate.Infrastructure.Repositories.Brokers;
using RealEstate.Infrastructure.Mongo.Indexes;
using RealEstate.Infrastructure.Mongo.Conventions;
using RealEstate.Application.Mapping;
using RealEstate.Application.Features.Leads.Contracts;
using RealEstate.Application.Features.Leads.Services;
using RealEstate.Application.Features.Properties.Contracts;
using RealEstate.Application.Features.Properties.Services;
using RealEstate.Infrastructure.Repositories.Leads;
using RealEstate.Infrastructure.Repositories.Properties;
using RealEstate.Infrastructure.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true)
        );
    });


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(RealEstate.Application.Features.Leads.Create.CreateLeadRequestValidator).Assembly);

builder.Services.AddAutoMapper(typeof(RealEstate.Application.Features.Brokers.Mapping.BrokerMappingProfile).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.ExampleFilters(); // turn on filters
	c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    // Optional but nice to have: show full type names less often, keep schemas clean
    c.SupportNonNullableReferenceTypes();
});
builder.Services.AddSwaggerExamplesFromAssemblies(typeof(Program).Assembly);

// Mongo options
builder.Services.AddOptions<MongoOptions>()
    .Bind(builder.Configuration.GetSection(MongoOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "Mongo:ConnectionString is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Database), "Mongo:Database is required")
    .ValidateOnStart();

MongoConventions.Register();

// Mongo DI
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    return new MongoClient(opt.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(opt.Database);
});

builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();

builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<ILeadService, LeadService>();

builder.Services.AddHostedService<MongoIndexInitializer>();
builder.Services.AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("Service is running."),
        tags: new[] { "live" })
    .AddCheck<MongoPingHealthCheck>(
        "mongo",
        tags: new[] { "ready" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// always 200
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

// keep it for now - some services still using it
// TODO: remove eventually since now I have proper k8s checks 
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

app.Run();