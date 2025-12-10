namespace Platemates.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ClerkUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? Location { get; set; }
    public string? Description { get; set; }

}