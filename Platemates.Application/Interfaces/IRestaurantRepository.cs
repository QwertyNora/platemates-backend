using Platemates.Domain.Entities;
using Platemates.Domain.Enums;

namespace Platemates.Application.Interfaces;

public interface IRestaurantRepository
{
    /// <summary>
    /// Find a restaurant by exact name and address match (case-insensitive)
    /// </summary>
    Task<Restaurant?> GetRestaurantByNameAndAddressAsync(string name, string address);

    /// <summary>
    /// Create a new restaurant
    /// </summary>
    Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);

    /// <summary>
    /// Get a user's restaurant relation by user ID and restaurant ID
    /// </summary>
    Task<UserRestaurant?> GetUserRestaurantAsync(Guid userId, Guid restaurantId);

    /// <summary>
    /// Get a user's restaurant by UserRestaurant ID (includes Restaurant and Review)
    /// </summary>
    Task<UserRestaurant?> GetUserRestaurantByIdAsync(Guid userRestaurantId, Guid userId);

    /// <summary>
    /// Create a new user-restaurant relation
    /// </summary>
    Task<UserRestaurant> CreateUserRestaurantAsync(UserRestaurant userRestaurant);

    /// <summary>
    /// Update a user-restaurant relation
    /// </summary>
    Task<UserRestaurant> UpdateUserRestaurantAsync(UserRestaurant userRestaurant);

    /// <summary>
    /// Get user restaurant with full restaurant details included
    /// </summary>
    Task<UserRestaurant?> GetUserRestaurantWithDetailsAsync(Guid userRestaurantId);

    /// <summary>
    /// Get all user's restaurants, optionally filtered by status, ordered by CreatedAt DESC
    /// </summary>
    Task<List<UserRestaurant>> GetUserRestaurantsAsync(Guid userId, RestaurantStatus? status = null);

    /// <summary>
    /// Create a review for a user restaurant
    /// </summary>
    Task<RestaurantReview> CreateReviewAsync(RestaurantReview review);

    /// <summary>
    /// Delete a review
    /// </summary>
    Task DeleteReviewAsync(RestaurantReview review);

    /// <summary>
    /// Update restaurant information
    /// </summary>
    Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);

    /// <summary>
    /// Delete a user's restaurant relationship
    /// </summary>
    Task DeleteUserRestaurantAsync(UserRestaurant userRestaurant);
}