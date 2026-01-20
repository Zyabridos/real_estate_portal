using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Services;
using RealEstate.Infrastructure.Mongo;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Infrastructure.Mongo.Indexes;
using RealEstate.Infrastructure.Mongo.Conventions;
using RealEstate.Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(RealEstate.Application.Validation.Leads.CreateLeadRequestValidator).Assembly);

//Mapper
builder.Services.AddAutoMapper(typeof(PropertyProfile).Assembly);

// JSON-converter (Enum -> String in my case)
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true)
        );
    });

// Swagger
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

// Mongo conventions
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

// Repos
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();

// Services
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<ILeadService, LeadService>();

// Index initialization
builder.Services.AddHostedService<MongoIndexInitializer>();

// ---- BUILD
var app = builder.Build();

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Health - TODO: move to Controllers
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
