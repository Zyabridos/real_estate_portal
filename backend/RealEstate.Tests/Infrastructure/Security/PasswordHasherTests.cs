namespace RealEstate.UnitTests.Infrastructure.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RealEstate.Domain.Entities.Users;
using RealEstate.Domain.Enums.Users;
using RealEstate.Infrastructure.Security;

public sealed class PasswordHasherTests
{
    [Fact]
    public void HashPassword_Should_ReturnHashDifferentFromOriginalPassword()
    {
        var sut = CreateSut(iterationCount: 100_000);
        var user = CreateUser();
        const string password = "StrongPassword123!";

        var hash = sut.HashPassword(user, password);

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void VerifyPassword_Should_ReturnTrue_ForValidPassword()
    {
        var sut = CreateSut(iterationCount: 100_000);
        var user = CreateUser();
        const string password = "StrongPassword123!";

        var hash = sut.HashPassword(user, password);

        var result = sut.VerifyPassword(user, hash, password);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_Should_ReturnFalse_ForInvalidPassword()
    {
        var sut = CreateSut(iterationCount: 100_000);
        var user = CreateUser();
        const string password = "StrongPassword123!";

        var hash = sut.HashPassword(user, password);

        var result = sut.VerifyPassword(user, hash, "WrongPassword!");

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_Should_ReturnTrue_WhenRehashIsNeeded()
    {
        var user = CreateUser();
        const string password = "StrongPassword123!";

        var oldHasher = CreateSut(iterationCount: 10_000);
        var newHasher = CreateSut(iterationCount: 100_000);

        var oldHash = oldHasher.HashPassword(user, password);

        var result = newHasher.VerifyPassword(user, oldHash, password);

        Assert.True(result);
    }

    private static UserPasswordHasher CreateSut(int iterationCount)
    {
        var options = Options.Create(new PasswordHasherOptions
        {
            IterationCount = iterationCount
        });

        return new UserPasswordHasher(options);
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = 1,
            Email = "admin@realestate.local",
            Role = UserRole.Admin
        };
    }
}