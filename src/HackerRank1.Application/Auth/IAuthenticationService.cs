using HackerRank1.DTO;

namespace HackerRank1.Services;

public interface IAuthenticationService
{
    Task<User> AuthenticateAsync(string email, string password);
}
