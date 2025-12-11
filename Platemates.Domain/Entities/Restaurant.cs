namespace Platemates.Domain.Entities;

public class Restaurant
{
    public int Id { get; set; }
    public string GooglePlaceId { get; set; } = string.Empty; // Unikt från Google
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public double? GoogleRating { get; set; }
    public int? PriceLevel { get; set; } // 1-4 från Google
    public string? PhotoUrl { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserRestaurant> UserRestaurants { get; set; }
}