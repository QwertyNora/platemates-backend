using Microsoft.EntityFrameworkCore;
using Platemates.Domain.Entities;

namespace Platemates.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<UserRestaurant> UserRestaurants { get; set; }
    public DbSet<RestaurantReview> RestaurantReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.ClerkUserId)
                  .IsUnique();

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(64);
        });

        // Restaurant configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasIndex(r => r.GooglePlaceId)
                  .IsUnique()
                  .HasFilter("\"GooglePlaceId\" IS NOT NULL"); // Unique only for non-null values

            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(r => r.Address)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(r => r.CuisineType)
                .HasMaxLength(128);

            entity.Property(r => r.PhoneNumber)
                .HasMaxLength(32);

            entity.Property(r => r.Website)
                .HasMaxLength(512);
        });

        // UserRestaurant configuration
        modelBuilder.Entity<UserRestaurant>(entity =>
        {
            entity.HasKey(ur => ur.Id);

            // Composite unique index: One user can only have one entry per restaurant
            entity.HasIndex(ur => new { ur.UserId, ur.RestaurantId })
                  .IsUnique();

            entity.Property(ur => ur.Status)
                .IsRequired()
                .HasConversion<int>(); // Store enum as int in database

            entity.Property(ur => ur.Notes)
                .HasMaxLength(1000);

            // Relationships
            entity.HasOne(ur => ur.User)
                  .WithMany()
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Restaurant)
                  .WithMany(r => r.UserRestaurants)
                  .HasForeignKey(ur => ur.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // RestaurantReview configuration
        modelBuilder.Entity<RestaurantReview>(entity =>
        {
            entity.HasKey(rr => rr.Id);

            entity.Property(rr => rr.Rating)
                .IsRequired();

            entity.Property(rr => rr.PriceRange)
                .IsRequired();

            entity.Property(rr => rr.Notes)
                .HasMaxLength(2000);

            // One-to-one relationship with UserRestaurant
            entity.HasOne(rr => rr.UserRestaurant)
                  .WithOne(ur => ur.Review)
                  .HasForeignKey<RestaurantReview>(rr => rr.UserRestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}