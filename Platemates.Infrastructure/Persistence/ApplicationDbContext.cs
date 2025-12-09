using Microsoft.EntityFrameworkCore;
using Platemates.Domain.Entities;

namespace Platemates.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}