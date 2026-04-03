using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using RealEstate.Application;
using RealEstate.Application.Features.Properties.Services;
using RealEstate.Application.Common.Abstractions;
using RealEstate.Infrastructure.DependencyInjection;
using RealEstate.Infrastructure.HealthChecks;
using RealEstate.Infrastructure.Mongo;
using RealEstate.Infrastructure.Mongo.Conventions;
using RealEstate.Infrastructure.Mongo.Indexes;
using RealEstate.Infrastructure.Persistence.Sequences;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using Swashbuckle.AspNetCore.Filters;

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
            .AddFeatureRegistrations()
            .AddApiHealthChecks();

        return services;
    }

    private static IServiceCollection AddApiMvc(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true)
                );
            });

        return services;
    }

    private static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        // Registers all FluentValidation validators from the Application layer assembly. PropertyService only to reference the correct assembly (assembly anchor).
        services.AddValidatorsFromAssembly(typeof(PropertyService).Assembly);

        return services;
    }
    
    private static IServiceCollection AddApiAutoMapper(this IServiceCollection services)
    {
        // Same: registrates all AutoMappers - PropertyService is used only as an assembly anchor.
        var assemblies = new[]
        {
            typeof(PropertyService).Assembly
        };

        services.AddSingleton(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assemblies);
            }, loggerFactory);

            var logger = sp.GetService<ILoggerFactory>()
                             ?.CreateLogger("AutoMapper") 
                         ?? NullLoggerFactory.Instance.CreateLogger("AutoMapper");

            return config;
        });

        services.AddSingleton<AutoMapper.IConfigurationProvider>(sp =>
            sp.GetRequiredService<AutoMapper.MapperConfiguration>()
        );

        services.AddSingleton<AutoMapper.IMapper>(sp =>
            sp.GetRequiredService<AutoMapper.MapperConfiguration>().CreateMapper(sp.GetService)
        );

        return services;
    }


    private static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.ExampleFilters();
            c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            c.SupportNonNullableReferenceTypes();
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
            var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            return new MongoClient(opt.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(opt.Database);
        });

        services.AddScoped<ISequenceGenerator, MongoSequenceGenerator>();
        services.AddHostedService<MongoIndexInitializer>();

        return services;
    }

    private static void RegisterMongoConventionsOnce()
    {
        if (Interlocked.Exchange(ref _mongoConventionsRegistered, 1) == 1)
            return;

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