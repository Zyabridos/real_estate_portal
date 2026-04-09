namespace RealEstate.Application.Common.Interfaces;

using RealEstate.Application.Common.Models;
using RealEstate.Domain.Entities.Users;

public interface ITokenService
{
    AccessTokenResult CreateAccessToken(User user);
}