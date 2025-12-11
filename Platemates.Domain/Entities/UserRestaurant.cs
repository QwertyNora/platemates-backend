using Platemates.Domain.Enums;

namespace Platemates.Domain.Entities;

public class UserRestaurant
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    public RestaurantStatus Status { get; set; } = RestaurantStatus.WantToGo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public RestaurantReview? Review { get; set; } // Null när WantToGo
}