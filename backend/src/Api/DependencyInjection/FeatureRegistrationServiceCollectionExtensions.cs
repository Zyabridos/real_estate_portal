using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Features.Properties.Services;
using RealEstate.Infrastructure.Mongo.Indexes;

namespace RealEstate.Api.DependencyInjection;

public static class FeatureRegistrationServiceCollectionExtensions
{
    // https://codewithmukesh.com/blog/scrutor-dotnet-auto-register-dependencies/
    public static IServiceCollection AddFeatureRegistrations(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(typeof(PropertyService).Assembly)
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
            .AsMatchingInterface()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(typeof(MongoIndexInitializer).Assembly)
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
            .AsMatchingInterface()
            .WithScopedLifetime());

        return services;
    }
}