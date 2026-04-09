namespace RealEstate.Infrastructure.Repositories.Users;

using MongoDB.Driver;
using RealEstate.Application.Common.Abstractions;
using RealEstate.Application.Features.Users.Contracts;
using RealEstate.Domain.Entities.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly ISequenceGenerator _sequenceGenerator;

    public UserRepository(IMongoDatabase database, ISequenceGenerator sequenceGenerator)
    {
        _users = database.GetCollection<User>("users");
        _sequenceGenerator = sequenceGenerator;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _users
            .Find(user => user.Email == email)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _users
            .Find(user => user.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<User> CreateAsync(User user, CancellationToken ct)
    {
        if (user.Id <= 0)
        {
            user.Id = await _sequenceGenerator.GetNextValueAsync("users", ct);
        }

        await _users.InsertOneAsync(user, cancellationToken: ct);
        return user;
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken ct)
    {
        var result = await _users.ReplaceOneAsync(
            existing => existing.Id == user.Id,
            user,
            cancellationToken: ct);

        return result.MatchedCount == 0 ? null : user;
    }
}