using Platemates.Application.Dtos;
using Platemates.Application.Interfaces;
using Platemates.Domain.Entities;
using Platemates.Domain.Enums;

namespace Platemates.Application.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _repository;

    public RestaurantService(IRestaurantRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserRestaurantDto> AddRestaurantManuallyAsync(Guid userId, AddRestaurantManuallyDto dto)
    {
        // Check if restaurant exists globally (by name + address)
        var existingRestaurant = await _repository.GetRestaurantByNameAndAddressAsync(dto.Name, dto.Address);

        Restaurant restaurant;

        if (existingRestaurant != null)
        {
            // Restaurant exists, check if user already has it
            var existingUserRestaurant = await _repository.GetUserRestaurantAsync(userId, existingRestaurant.Id);

            if (existingUserRestaurant != null)
            {
                // User already has this restaurant - return existing
                return MapToUserRestaurantDto(existingUserRestaurant);
            }

            restaurant = existingRestaurant;
        }
        else
        {
            // Create new restaurant (manually added, no Google data)
            restaurant = new Restaurant
            {
                GooglePlaceId = null, // Manually added
                Name = dto.Name,
                Address = dto.Address,
                CuisineType = dto.CuisineType,
                Latitude = null, // No coordinates for manual entry
                Longitude = null
            };

            restaurant = await _repository.CreateRestaurantAsync(restaurant);
        }

        // Create UserRestaurant relation
        var userRestaurant = new UserRestaurant
        {
            UserId = userId,
            RestaurantId = restaurant.Id,
            Status = RestaurantStatus.WantToGo,
            Notes = dto.Notes
        };

        userRestaurant = await _repository.CreateUserRestaurantAsync(userRestaurant);

        return MapToUserRestaurantDto(userRestaurant);
    }

    private UserRestaurantDto MapToUserRestaurantDto(UserRestaurant userRestaurant)
    {
        return new UserRestaurantDto(
            userRestaurant.Id,
            new RestaurantDto(
                userRestaurant.Restaurant.Id,
                userRestaurant.Restaurant.GooglePlaceId,
                userRestaurant.Restaurant.Name,
                userRestaurant.Restaurant.Address,
                userRestaurant.Restaurant.CuisineType,
                userRestaurant.Restaurant.PhoneNumber,
                userRestaurant.Restaurant.Website,
                userRestaurant.Restaurant.Latitude,
                userRestaurant.Restaurant.Longitude,
                userRestaurant.Restaurant.CreatedAt
            ),
            userRestaurant.Status.ToString(),
            userRestaurant.Notes,
            userRestaurant.CreatedAt,
            userRestaurant.UpdatedAt,
            null // No review for WantToGo status
        );
    }
}