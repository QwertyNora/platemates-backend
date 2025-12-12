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
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
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

    public async Task<List<UserRestaurantDto>> GetMyRestaurantsAsync(Guid userId, string status = "all")
    {
        RestaurantStatus? statusFilter = status.ToLower() switch
        {
            "want-to-go" => RestaurantStatus.WantToGo,
            "been-to" => RestaurantStatus.BeenTo,
            _ => null // "all" or any other value = no filter
        };

        var userRestaurants = await _repository.GetUserRestaurantsAsync(userId, statusFilter);

        return userRestaurants.Select(MapToUserRestaurantDto).ToList();
    }

    public async Task<UserRestaurantDto> MarkAsBeenToAsync(Guid userId, Guid userRestaurantId, AddReviewDto reviewDto)
    {
        // Get user restaurant (validate ownership)
        var userRestaurant = await _repository.GetUserRestaurantByIdAsync(userRestaurantId, userId);

        if (userRestaurant == null)
        {
            throw new InvalidOperationException("Restaurant not found or you don't have access to it");
        }

        if (userRestaurant.Status == RestaurantStatus.BeenTo)
        {
            throw new InvalidOperationException("Restaurant is already marked as 'Been To'");
        }

        // Create review
        var review = new RestaurantReview
        {
            UserRestaurantId = userRestaurantId,
            Rating = reviewDto.Rating,
            PriceRange = reviewDto.PriceRange,
            Notes = reviewDto.Notes
        };

        await _repository.CreateReviewAsync(review);

        // Update status
        userRestaurant.Status = RestaurantStatus.BeenTo;
        userRestaurant = await _repository.UpdateUserRestaurantAsync(userRestaurant);

        return MapToUserRestaurantDto(userRestaurant);
    }

    public async Task<UserRestaurantDto> MarkAsWantToGoAsync(Guid userId, Guid userRestaurantId)
    {
        // Get user restaurant (validate ownership)
        var userRestaurant = await _repository.GetUserRestaurantByIdAsync(userRestaurantId, userId);

        if (userRestaurant == null)
        {
            throw new InvalidOperationException("Restaurant not found or you don't have access to it");
        }

        if (userRestaurant.Status == RestaurantStatus.WantToGo)
        {
            throw new InvalidOperationException("Restaurant is already marked as 'Want to Go'");
        }

        // Delete review if it exists
        if (userRestaurant.Review != null)
        {
            await _repository.DeleteReviewAsync(userRestaurant.Review);
        }

        // Update status
        userRestaurant.Status = RestaurantStatus.WantToGo;
        userRestaurant = await _repository.UpdateUserRestaurantAsync(userRestaurant);

        return MapToUserRestaurantDto(userRestaurant);
    }

    public async Task<UserRestaurantDto> UpdateRestaurantAsync(Guid userId, Guid userRestaurantId, UpdateRestaurantDto dto)
    {
        // Get user restaurant (validate ownership)
        var userRestaurant = await _repository.GetUserRestaurantByIdAsync(userRestaurantId, userId);

        if (userRestaurant == null)
        {
            throw new InvalidOperationException("Restaurant not found or you don't have access to it");
        }

        var restaurant = userRestaurant.Restaurant;

        // PATCH: Only update fields that are provided (not null)
        if (dto.Name != null)
        {
            restaurant.Name = dto.Name;
        }

        if (dto.Address != null)
        {
            restaurant.Address = dto.Address;
        }

        if (dto.CuisineType != null)
        {
            restaurant.CuisineType = dto.CuisineType;
        }

        // Update restaurant
        await _repository.UpdateRestaurantAsync(restaurant);

        // Update UserRestaurant notes if provided
        if (dto.Notes != null)
        {
            userRestaurant.Notes = dto.Notes;
            userRestaurant = await _repository.UpdateUserRestaurantAsync(userRestaurant);
        }

        return MapToUserRestaurantDto(userRestaurant);
    }

    public async Task DeleteRestaurantAsync(Guid userId, Guid userRestaurantId)
    {
        var userRestaurant = await _repository.GetUserRestaurantByIdAsync(userRestaurantId, userId);

        if (userRestaurant == null)
        {
            throw new InvalidOperationException("Restaurant not found or you don't have permission to delete it.");
        }

        await _repository.DeleteUserRestaurantAsync(userRestaurant);
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
            userRestaurant.Review != null
                ? new RestaurantReviewDto(
                    userRestaurant.Review.Id,
                    userRestaurant.Review.Rating,
                    userRestaurant.Review.PriceRange,
                    userRestaurant.Review.Notes,
                    userRestaurant.Review.CreatedAt
                )
                : null
        );
    }
}