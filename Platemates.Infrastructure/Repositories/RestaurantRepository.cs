using Microsoft.EntityFrameworkCore;
using Platemates.Application.Interfaces;
using Platemates.Domain.Entities;
using Platemates.Domain.Enums;
using Platemates.Infrastructure.Persistence;

namespace Platemates.Infrastructure.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly ApplicationDbContext _context;

    public RestaurantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant?> GetRestaurantByNameAndAddressAsync(string name, string address)
    {
        return await _context.Restaurants
            .FirstOrDefaultAsync(r =>
                r.Name.ToLower() == name.ToLower() &&
                r.Address.ToLower() == address.ToLower());
    }

    public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
    {
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }

    public async Task<UserRestaurant?> GetUserRestaurantAsync(Guid userId, Guid restaurantId)
    {
        return await _context.UserRestaurants
            .Include(ur => ur.Restaurant)
            .FirstOrDefaultAsync(ur =>
                ur.UserId == userId &&
                ur.RestaurantId == restaurantId);
    }

    public async Task<UserRestaurant?> GetUserRestaurantByIdAsync(Guid userRestaurantId, Guid userId)
    {
        return await _context.UserRestaurants
            .Include(ur => ur.Restaurant)
            .Include(ur => ur.Review)
            .FirstOrDefaultAsync(ur =>
                ur.Id == userRestaurantId &&
                ur.UserId == userId);
    }

    public async Task<UserRestaurant> CreateUserRestaurantAsync(UserRestaurant userRestaurant)
    {
        _context.UserRestaurants.Add(userRestaurant);
        await _context.SaveChangesAsync();

        // Load the restaurant relation
        await _context.Entry(userRestaurant)
            .Reference(ur => ur.Restaurant)
            .LoadAsync();

        return userRestaurant;
    }

    public async Task<UserRestaurant> UpdateUserRestaurantAsync(UserRestaurant userRestaurant)
    {
        userRestaurant.UpdatedAt = DateTime.UtcNow;
        _context.UserRestaurants.Update(userRestaurant);
        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(userRestaurant)
            .Reference(ur => ur.Restaurant)
            .LoadAsync();

        await _context.Entry(userRestaurant)
            .Reference(ur => ur.Review)
            .LoadAsync();

        return userRestaurant;
    }

    public async Task<UserRestaurant?> GetUserRestaurantWithDetailsAsync(Guid userRestaurantId)
    {
        return await _context.UserRestaurants
            .Include(ur => ur.Restaurant)
            .FirstOrDefaultAsync(ur => ur.Id == userRestaurantId);
    }

    public async Task<List<UserRestaurant>> GetUserRestaurantsAsync(Guid userId, RestaurantStatus? status = null)
    {
        var query = _context.UserRestaurants
            .Include(ur => ur.Restaurant)
            .Include(ur => ur.Review) // Include reviews for "Been To" restaurants
            .Where(ur => ur.UserId == userId);

        // Filter by status if provided
        if (status.HasValue)
        {
            query = query.Where(ur => ur.Status == status.Value);
        }

        // Order by newest first
        query = query.OrderByDescending(ur => ur.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<RestaurantReview> CreateReviewAsync(RestaurantReview review)
    {
        _context.RestaurantReviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task DeleteReviewAsync(RestaurantReview review)
    {
        _context.RestaurantReviews.Remove(review);
        await _context.SaveChangesAsync();
    }

    public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }
}