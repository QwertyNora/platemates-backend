using Platemates.Application.Dtos;

namespace Platemates.Application.Interfaces;

public interface IRestaurantService
{
    /// <summary>
    /// Add a restaurant manually to the user's "Want to Go" list
    /// </summary>
    Task<UserRestaurantDto> AddRestaurantManuallyAsync(Guid userId, AddRestaurantManuallyDto dto);
}