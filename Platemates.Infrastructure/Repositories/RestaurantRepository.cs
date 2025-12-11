using Microsoft.EntityFrameworkCore;
using Platemates.Application.Interfaces;
using Platemates.Domain.Entities;
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

    public async Task<UserRestaurant?> GetUserRestaurantWithDetailsAsync(Guid userRestaurantId)
    {
        return await _context.UserRestaurants
            .Include(ur => ur.Restaurant)
            .FirstOrDefaultAsync(ur => ur.Id == userRestaurantId);
    }
}