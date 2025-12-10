namespace Platemates.Application.Dtos;

public record UserResponseDto(
    Guid Id,
    string ClerkUserId,
    string Email,
    string Username,
    DateTime CreatedAt,
    string? Location,
    string? Description
);
