// Platemates.Domain/Entities/UserRestaurant.cs
using Platemates.Domain.Enums;

namespace Platemates.Domain.Entities;

public class UserRestaurant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relations
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    // Status & Data
    public RestaurantStatus Status { get; set; } = RestaurantStatus.WantToGo;
    public string? Notes { get; set; } // User's personal notes about why they want to go

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation (for future)
    public RestaurantReview? Review { get; set; }
}