using Platemates.Domain.Entities;

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
    /// Create a new user-restaurant relation
    /// </summary>
    Task<UserRestaurant> CreateUserRestaurantAsync(UserRestaurant userRestaurant);

    /// <summary>
    /// Get user restaurant with full restaurant details included
    /// </summary>
    Task<UserRestaurant?> GetUserRestaurantWithDetailsAsync(Guid userRestaurantId);
}