using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// --- Mongo config
var mongoConnectionString =
    builder.Configuration["MONGODB:CONNECTION_STRING"]
    ?? throw new InvalidOperationException("MONGODB:CONNECTION_STRING is not set");

var mongoDatabaseName =
    builder.Configuration["MONGODB:DATABASE"]
    ?? throw new InvalidOperationException("MONGODB:DATABASE is not set");

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(mongoConnectionString));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

// --- App
var app = builder.Build();

app.MapGet("/api/health", async (IMongoDatabase db, IWebHostEnvironment env) =>
{
    // Ping MongoDB
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