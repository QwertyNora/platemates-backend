using Platemates.Application.Dtos;
using Platemates.Application.Interfaces;
using Platemates.Domain.Entities;

namespace Platemates.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserResponseDto> GetOrCreateUserAsync(
        Guid Id,
        string ClerkUserId,
        string Email,
        string Username,
        DateTime CreatedAt,
        string? Location,
        string? Description
    )
    {
        var user = await _repo.GetUserByClerkIdAsync(ClerkUserId);
        Console.WriteLine("__________INSIDE THE USER SERVICE__________");
        Console.WriteLine("_____user: " + user);
        if (user == null)
        {
            user = await _repo.CreateUserAsync(
                new User
                {
                    ClerkUserId = ClerkUserId,
                    Email = Email,
                    Username = Username,
                    Location = Location,
                    Description = Description
                }
            );
        }

        return new UserResponseDto(
            user.Id,
            user.ClerkUserId,
            user.Email,
            user.Username,
            user.CreatedAt,
            user.Location,
            user.Description
        );
    }
}
