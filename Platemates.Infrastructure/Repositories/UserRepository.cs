using Microsoft.EntityFrameworkCore;
using Platemates.Application.Interfaces;
using Platemates.Domain.Entities;
using Platemates.Infrastructure.Persistence;


namespace Platemates.Infrastructure.Repositories;

public class UserRepo : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByClerkIdAsync(string clerkUserId)
    {
        return await _context.Users.FirstOrDefaultAsync(user =>
            user.ClerkUserId == clerkUserId
        );
    }
}
