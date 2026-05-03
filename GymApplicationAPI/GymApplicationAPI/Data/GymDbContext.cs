using GymApplicationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GymApplicationAPI.Data;

public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Booking - unique constraint so a user can't book the same session twice
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(b => new { b.UserId, b.SessionId }).IsUnique();

            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Session)
                  .WithMany(s => s.Bookings)
                  .HasForeignKey(b => b.SessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}