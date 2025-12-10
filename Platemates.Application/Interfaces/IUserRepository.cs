using Platemates.Domain.Entities;

namespace Platemates.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByClerkIdAsync(string clerkUserId);
    Task<User> CreateUserAsync(User user);
}