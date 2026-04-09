namespace RealEstate.Infrastructure.DependencyInjection;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Features.Users.Contracts;
using RealEstate.Infrastructure.Persistence.Seed;
using RealEstate.Infrastructure.Repositories.Users;
using RealEstate.Infrastructure.Security;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer =
                configuration["JWT_ISSUER"]
                ?? configuration["Jwt:Issuer"]
                ?? string.Empty;

            options.Audience =
                configuration["JWT_AUDIENCE"]
                ?? configuration["Jwt:Audience"]
                ?? string.Empty;

            options.SigningKey =
                configuration["JWT_SIGNING_KEY"]
                ?? configuration["Jwt:SigningKey"]
                ?? string.Empty;

            var accessTokenMinutesText =
                configuration["JWT_ACCESS_TOKEN_MINUTES"]
                ?? configuration["Jwt:AccessTokenMinutes"];

            options.AccessTokenMinutes =
                int.TryParse(accessTokenMinutesText, out var parsedMinutes)
                    ? parsedMinutes
                    : 60;
        });

        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = 100_000;
        });

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IUserPasswordHasher, UserPasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<AdminUserSeeder>();

        return services;
    }
}