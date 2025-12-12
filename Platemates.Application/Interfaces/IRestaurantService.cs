using Platemates.Application.Dtos;

namespace Platemates.Application.Interfaces;

public interface IRestaurantService
{
    /// <summary>
    /// Add a restaurant manually to the user's "Want to Go" list
    /// </summary>
    Task<UserRestaurantDto> AddRestaurantManuallyAsync(Guid userId, AddRestaurantManuallyDto dto);

    /// <summary>
    /// Get user's restaurant filtered by status
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="status">"all", "want-to-go", or "been-to"</param>
    Task<List<UserRestaurantDto>> GetMyRestaurantsAsync(Guid userId, string status = "all");

    /// <summary>
    /// Mark a restaurant as "Been To" and add a review
    /// </summary>
    Task<UserRestaurantDto> MarkAsBeenToAsync(Guid userId, Guid userRestaurantId, AddReviewDto reviewDto);

    /// <summary>
    /// Mark a restaurant back to "Want to Go" and delete the review
    /// </summary>
    Task<UserRestaurantDto> MarkAsWantToGoAsync(Guid userId, Guid userRestaurantId);

    /// <summary>
    /// Update restaurant information and notes (PATCH - only updates provided fields)
    /// </summary>
    Task<UserRestaurantDto> UpdateRestaurantAsync(Guid userId, Guid userRestaurantId, UpdateRestaurantDto dto);

    /// <summary>
    /// Delete a restaurant from user's list
    /// </summary>
    Task DeleteRestaurantAsync(Guid userId, Guid userRestaurantId);
}