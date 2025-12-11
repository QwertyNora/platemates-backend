namespace Platemates.Domain.Entities;

public class RestaurantReview
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserRestaurantId { get; set; }
    public UserRestaurant UserRestaurant { get; set; } = null!;

    // Review data 
    public int Rating { get; set; } // 1-5
    public int PriceRange { get; set; } // 1-4
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}