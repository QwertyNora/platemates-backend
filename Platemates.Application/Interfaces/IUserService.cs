

using Platemates.Application.Dtos;

namespace Platemates.Application.Interfaces;

public interface IUserService
{
    public Task<UserResponseDto> GetOrCreateUserAsync(
        Guid Id,
        string ClerkUserId,
        string Email,
        string Username,
        DateTime CreatedAt,
        string? Location,
        string? Description
    );
}
