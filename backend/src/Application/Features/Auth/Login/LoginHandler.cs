namespace RealEstate.Application.Features.Auth.Login;

using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Features.Users.Contracts;

public sealed class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        IUserRepository userRepository,
        IUserPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> HandleAsync(LoginRequest request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, ct);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var isValidPassword = _passwordHasher.VerifyPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = _tokenService.CreateAccessToken(user);

        return new LoginResponse(token.Token, token.ExpiresAtUtc);
    }
}