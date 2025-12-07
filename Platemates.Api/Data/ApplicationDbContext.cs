using Microsoft.EntityFrameworkCore;
using Platemates.Api.Models;

namespace Platemates.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
}