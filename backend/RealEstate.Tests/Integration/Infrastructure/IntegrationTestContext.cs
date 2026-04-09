using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RealEstate.TestData.Fixtures;
using RealEstate.TestData.Mongo;
using RealEstate.Infrastructure.Persistence.Seed;

namespace RealEstate.Tests.Integration.Infrastructure;

public sealed class IntegrationTestContext
{
    private const string TestJwtIssuer = "RealEstate.Tests";
    private const string TestJwtAudience = "RealEstate.Tests.Client";
    private const string TestJwtSigningKey = "super-secret-test-signing-key-1234567890";
    private const string TestJwtAccessTokenMinutes = "60";

    private const string TestAdminEmail = "admin@realestate.local";
    private const string TestAdminPassword = "Admin12345!";

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient? _adminClient;

    public MongoDbFixture Fixture { get; }
    public HttpClient Client { get; }

    public IntegrationTestContext(MongoDbFixture fixture)
    {
        Fixture = fixture;

        if (string.IsNullOrWhiteSpace(fixture.ConnectionString))
        {
            throw new InvalidOperationException("MongoDbFixture.ConnectionString is empty.");
        }

        if (string.IsNullOrWhiteSpace(fixture.DatabaseName))
        {
            throw new InvalidOperationException("MongoDbFixture.DatabaseName is empty.");
        }

        Environment.SetEnvironmentVariable("Mongo__ConnectionString", fixture.ConnectionString);
        Environment.SetEnvironmentVariable("Mongo__Database", fixture.DatabaseName);
        Environment.SetEnvironmentVariable("Mongo__DatabaseName", fixture.DatabaseName);

        Environment.SetEnvironmentVariable("JWT_ISSUER", TestJwtIssuer);
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", TestJwtAudience);
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", TestJwtSigningKey);
        Environment.SetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES", TestJwtAccessTokenMinutes);

        Environment.SetEnvironmentVariable("AUTH_SEED_ADMIN_EMAIL", TestAdminEmail);
        Environment.SetEnvironmentVariable("AUTH_SEED_ADMIN_PASSWORD", TestAdminPassword);

        _factory = new CustomWebApplicationFactory(
            fixture.Database,
            fixture.ConnectionString,
            fixture.DatabaseName
        );

        Client = _factory.CreateClient();
    }

    public async Task<HttpClient> GetAdminClientAsync()
	{
    	if (_adminClient is not null)
    	{
        	return _adminClient;
    	}

    	await EnsureAdminSeededAsync();

    	var client = _factory.CreateClient();

    	var loginResponse = await client.PostAsJsonAsync(
        	"/api/auth/login",
        	new
        	{
        	    email = TestAdminEmail,
    	        password = TestAdminPassword
	        });

    	loginResponse.EnsureSuccessStatusCode();

    	using var document = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
    	var token = ReadToken(document.RootElement);

    	client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    	_adminClient = client;

    	return _adminClient;
	}

	private async Task EnsureAdminSeededAsync()
	{
    	using var scope = _factory.Services.CreateScope();
    	var seeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
    	await seeder.SeedAsync(CancellationToken.None);
	}

    private static string ReadToken(JsonElement root)
    {
        if (root.TryGetProperty("accessToken", out var accessTokenElement))
        {
            var accessToken = accessTokenElement.GetString();

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                return accessToken;
            }
        }

        if (root.TryGetProperty("token", out var tokenElement))
        {
            var token = tokenElement.GetString();

            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }
        }

        throw new InvalidOperationException(
            "Login response does not contain accessToken or token.");
    }

    public IMongoCollection<T> Collection<T>(string name) =>
        Fixture.Database.GetCollection<T>(name);

    public IMongoCollection<T> GetCollection<T>(string name) =>
        Collection<T>(name);
}
