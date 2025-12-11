namespace Platemates.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Google Places data (null for manual entries)
    public string? GooglePlaceId { get; set; }

    // Core info (always required)
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Optional info
    public string? CuisineType { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }

    // Location (null for manual entries until geocoded)
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserRestaurant> UserRestaurants { get; set; } = new List<UserRestaurant>();
}