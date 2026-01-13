using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using FluentValidation;
using FluentValidation.AspNetCore;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Infrastructure.Mongo;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Infrastructure.Mongo.Indexes;
using RealEstate.Infrastructure.Mongo.Conventions;
using RealEstate.Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// FluentValidation registration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(RealEstate.Application.Validation.Leads.CreateLeadRequestValidator).Assembly);

// MongoDB registration
builder.Services.AddOptions<MongoOptions>()
    .Bind(builder.Configuration.GetSection(MongoOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "Mongo:ConnectionString is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Database), "Mongo:Database is required")
    .ValidateOnStart(); // => and drop the app if fails

// Mongo config
MongoConventions.Register();

// Mongo DI
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    return new MongoClient(opt.ConnectionString);
});

// Scoped
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(opt.Database);
});

// Repositories
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();

// Index initialization (HostedService)
builder.Services.AddHostedService<MongoIndexInitializer>();

// --- App
var app = builder.Build();

app.MapControllers();
// Health check with pinging of MongoDB
app.MapGet("/api/health", async (IMongoDatabase db, IWebHostEnvironment env) =>
{
    var command = new BsonDocument("ping", 1);
    await db.RunCommandAsync<BsonDocument>(command);
    
    return Results.Json(new
    {
        status = "ok",
        service = "realestate-api",
        environment = env.EnvironmentName,
        mongo = "ok"
    });
});

app.Run();