namespace RealEstate.Infrastructure.Persistence.Seed;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Features.Users.Contracts;
using RealEstate.Domain.Entities.Users;
using RealEstate.Domain.Enums.Users;

public sealed class AdminUserSeeder
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IUserPasswordHasher _passwordHasher;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        IConfiguration configuration,
        IUserRepository userRepository,
        IUserPasswordHasher passwordHasher,
        ILogger<AdminUserSeeder> logger)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        var email = _configuration["AUTH_SEED_ADMIN_EMAIL"]?.Trim().ToLowerInvariant();
        var password = _configuration["AUTH_SEED_ADMIN_PASSWORD"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogInformation(
                "Admin seed skipped because AUTH_SEED_ADMIN_EMAIL or AUTH_SEED_ADMIN_PASSWORD is not configured.");

            return;
        }

        var existingUser = await _userRepository.GetByEmailAsync(email, ct);

        if (existingUser is not null)
        {
            _logger.LogInformation("Admin seed skipped because admin user already exists: {Email}", email);
            return;
        }

        var user = new User
        {
            Email = email,
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _userRepository.CreateAsync(user, ct);

        _logger.LogInformation("Seeded admin user: {Email}", email);
    }
}