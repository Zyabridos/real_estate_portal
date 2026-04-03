namespace RealEstate.Application.Common.Interfaces;

using RealEstate.Domain.Entities.Users;

public interface IUserPasswordHasher
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string passwordHash, string providedPassword);
}