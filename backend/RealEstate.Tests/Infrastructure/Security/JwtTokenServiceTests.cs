namespace RealEstate.UnitTests.Infrastructure.Security;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Common.Security;
using RealEstate.Domain.Entities.Users;
using RealEstate.Domain.Enums.Users;
using RealEstate.Infrastructure.Security;

public sealed class JwtTokenServiceTests
{
    private static readonly DateTimeOffset FixedNow =
        new(2026, 4, 3, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateAccessToken_Should_ReturnJwt_AndExpectedExpiration()
    {
        var options = CreateOptions(accessTokenMinutes: 60);
        var sut = CreateSut(options);
        var user = CreateBrokerUser();

        var result = sut.CreateAccessToken(user);

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(FixedNow.AddMinutes(60), result.ExpiresAtUtc);
    }

    [Fact]
    public void CreateAccessToken_Should_ContainRequiredClaims()
    {
        var options = CreateOptions();
        var sut = CreateSut(options);
        var user = CreateBrokerUser();

        var result = sut.CreateAccessToken(user);
        var principal = ValidateToken(result.Token, options);

        Assert.Equal("42", principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        Assert.Equal("42", principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("broker@realestate.local", principal.FindFirstValue(JwtRegisteredClaimNames.Email));
        Assert.Equal("broker@realestate.local", principal.FindFirstValue(ClaimTypes.Email));
        Assert.Equal(UserRole.Broker.ToString(), principal.FindFirstValue(ClaimTypes.Role));
    }

    [Fact]
    public void CreateAccessToken_Should_ContainAgencyId_AndBrokerId_WhenPresent()
    {
        var options = CreateOptions();
        var sut = CreateSut(options);
        var user = CreateBrokerUser();

        var result = sut.CreateAccessToken(user);
        var principal = ValidateToken(result.Token, options);

        Assert.Equal("10", principal.FindFirstValue(CustomClaimTypes.AgencyId));
        Assert.Equal("77", principal.FindFirstValue(CustomClaimTypes.BrokerId));
    }

    [Fact]
    public void CreateAccessToken_Should_NotContainAgencyOrBrokerClaims_WhenAbsent()
    {
        var options = CreateOptions();
        var sut = CreateSut(options);

        var user = new User
        {
            Id = 1,
            Email = "admin@realestate.local",
            Role = UserRole.Admin
        };

        var result = sut.CreateAccessToken(user);
        var principal = ValidateToken(result.Token, options);

        Assert.Null(principal.FindFirst(CustomClaimTypes.AgencyId));
        Assert.Null(principal.FindFirst(CustomClaimTypes.BrokerId));
    }

    private static JwtTokenService CreateSut(JwtOptions options)
    {
        return new JwtTokenService(
            Options.Create(options),
            new FixedTimeProvider(FixedNow));
    }

    private static JwtOptions CreateOptions(int accessTokenMinutes = 60)
    {
        return new JwtOptions
        {
            Issuer = "RealEstatePortal",
            Audience = "RealEstatePortal.Client",
            SigningKey = "super-secret-test-key-change-me-please-12345",
            AccessTokenMinutes = accessTokenMinutes
        };
    }

    private static User CreateBrokerUser()
    {
        return new User
        {
            Id = 42,
            Email = "broker@realestate.local",
            Role = UserRole.Broker,
            AgencyId = 10,
            BrokerId = 77
        };
    }

    private static ClaimsPrincipal ValidateToken(string token, JwtOptions options)
    {
        var handler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,

            ValidateAudience = true,
            ValidAudience = options.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(options.SigningKey)),

            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,

            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };

        return handler.ValidateToken(token, validationParameters, out _);
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}