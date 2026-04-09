namespace RealEstate.Infrastructure.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Entities.Users;

public sealed class UserPasswordHasher : IUserPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher;

    public UserPasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _passwordHasher = new PasswordHasher<User>(options);
    }

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}