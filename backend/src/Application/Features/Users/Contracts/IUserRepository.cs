namespace RealEstate.Application.Features.Users.Contracts;

using RealEstate.Domain.Entities.Users;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<User> CreateAsync(User user, CancellationToken ct);
    Task<User?> UpdateAsync(User user, CancellationToken ct);
}