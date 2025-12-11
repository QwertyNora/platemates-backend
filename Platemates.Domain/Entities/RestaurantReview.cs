using Platemates.Domain.Entities;

namespace Platemates.Domain.Entities;

public class RestaurantReview
{
    public int Id { get; set; }
    public int UserRestaurantId { get; set; }
    public UserRestaurant UserRestaurant { get; set; } = null!;

    public int Rating { get; set; } // 1-5
    public int PriceRange { get; set; } // 1-4 kr
    public string? Notes { get; set; }
    public List<string>? PhotoUrls { get; set; } = new(); // JSON i databasen
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}