using System.ComponentModel.DataAnnotations;

namespace Platemates.Application.Dtos;

/// <summary>
/// DTO for manually adding a restaurant
/// </summary>
public record AddRestaurantManuallyDto(
    [Required(ErrorMessage = "Restaurant name is required")]
    [MaxLength(256, ErrorMessage = "Restaurant name cannot exceed 256 characters")]
    string Name,

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(512, ErrorMessage = "Address cannot exceed 512 characters")]
    string Address,

    [MaxLength(128, ErrorMessage = "Cuisine type cannot exceed 128 characters")]
    string? CuisineType,

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    string? Notes
);

/// <summary>
/// Response DTO for a Restaurant
/// </summary>
public record RestaurantDto(
    Guid Id,
    string? GooglePlaceId,
    string Name,
    string Address,
    string? CuisineType,
    string? PhoneNumber,
    string? Website,
    double? Latitude,
    double? Longitude,
    DateTime CreatedAt
);

/// <summary>
/// Response DTO for a UserRestaurant (user's relationship with a restaurant)
/// </summary>
public record UserRestaurantDto(
    Guid Id,
    RestaurantDto Restaurant,
    string Status, // "WantToGo" or "BeenTo"
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    RestaurantReviewDto? Review
);

/// <summary>
/// Response DTO for a RestaurantReview (only present when Status is "BeenTo")
/// </summary>
public record RestaurantReviewDto(
    Guid Id,
    int Rating,
    int PriceRange,
    string? Notes,
    DateTime CreatedAt
);