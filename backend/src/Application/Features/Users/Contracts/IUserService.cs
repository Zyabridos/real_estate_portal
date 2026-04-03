namespace RealEstate.Application.Features.Users.Contracts;

using RealEstate.Application.Features.Auth.Login;

public interface IUserService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct);
}