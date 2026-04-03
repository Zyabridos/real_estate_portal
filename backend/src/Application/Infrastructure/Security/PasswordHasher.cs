namespace RealEstate.Infrastructure.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Entities.Users;

public sealed class PasswordHasher : IUserPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _passwordHasher;

    public PasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>(options);
    }

    public string HashPassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password must not be empty.", nameof(password));
        }

        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string passwordHash, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash must not be empty.", nameof(passwordHash));
        }

        if (string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, providedPassword);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}