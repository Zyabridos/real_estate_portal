using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using RealEstate.Application.Common.Abstractions;
using RealEstate.Application.Features.Properties.Services;
using RealEstate.Domain.Enums.Users;
using RealEstate.Infrastructure.DependencyInjection;
using RealEstate.Infrastructure.HealthChecks;
using RealEstate.Infrastructure.Mongo;
using RealEstate.Infrastructure.Mongo.Conventions;
using RealEstate.Infrastructure.Mongo.Indexes;
using RealEstate.Infrastructure.Persistence.Sequences;
using RealEstate.Infrastructure.Security;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace RealEstate.Api.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    private static int _mongoConventionsRegistered;

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiMvc()
            .AddApiValidation()
            .AddApiAutoMapper()
            .AddApiSwagger()
            .AddMongo(configuration)
            .AddInfrastructure(configuration)
            .AddFeatureRegistrations()
            .AddApiAuthentication(configuration)
            .AddApiAuthorization()
            .AddApiHealthChecks();

        return services;
    }

    private static IServiceCollection AddApiMvc(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true));
            });

        return services;
    }

    private static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(PropertyService).Assembly);

        return services;
    }

    private static IServiceCollection AddApiAutoMapper(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(PropertyService).Assembly
        };

        services.AddSingleton(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assemblies);
            }, loggerFactory);

            return config;
        });

        services.AddSingleton<AutoMapper.IConfigurationProvider>(sp =>
            sp.GetRequiredService<MapperConfiguration>());

        services.AddSingleton<IMapper>(sp =>
            sp.GetRequiredService<MapperConfiguration>().CreateMapper(sp.GetService));

        return services;
    }

    private static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.ExampleFilters();
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            options.SupportNonNullableReferenceTypes();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT bearer token only"
            });

            options.AddSecurityRequirement(_ =>
            {
                OpenApiSecuritySchemeReference bearerReference = new("Bearer");

                return new OpenApiSecurityRequirement
                {
                    [bearerReference] = new List<string>()
                };
            });
        });

        services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MongoOptions>()
            .Bind(configuration.GetSection(MongoOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "Mongo:ConnectionString is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Database), "Mongo:Database is required")
            .ValidateOnStart();

        RegisterMongoConventionsOnce();

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            return new MongoClient(options.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.Database);
        });

        services.AddScoped<ISequenceGenerator, MongoSequenceGenerator>();
        services.AddHostedService<MongoIndexInitializer>();

        return services;
    }

    private static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions
        {
            Issuer = configuration["JWT_ISSUER"]
                     ?? configuration["Jwt:Issuer"]
                     ?? string.Empty,

            Audience = configuration["JWT_AUDIENCE"]
                       ?? configuration["Jwt:Audience"]
                       ?? string.Empty,

            SigningKey = configuration["JWT_SIGNING_KEY"]
                         ?? configuration["Jwt:SigningKey"]
                         ?? string.Empty,

            AccessTokenMinutes = int.TryParse(
                configuration["JWT_ACCESS_TOKEN_MINUTES"]
                ?? configuration["Jwt:AccessTokenMinutes"],
                out var accessTokenMinutes)
                ? accessTokenMinutes
                : 60
        };

        jwtOptions.Validate();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        return services;
    }

    private static IServiceCollection AddApiAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(UserRole.Admin.ToString());
            });
        });

        return services;
    }

    private static void RegisterMongoConventionsOnce()
    {
        if (Interlocked.Exchange(ref _mongoConventionsRegistered, 1) == 1)
        {
            return;
        }

        MongoConventions.Register();
    }

    private static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck(
                "self",
                () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Service is running."),
                tags: new[] { "live" })
            .AddCheck<MongoPingHealthCheck>(
                "mongo",
                tags: new[] { "ready" });

        return services;
    }
}