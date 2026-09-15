using Microsoft.EntityFrameworkCore;
using RideShare.Domain;

namespace RideShare.Infrastructure;

public class RideShareDbContext : DbContext
{
    public RideShareDbContext(DbContextOptions<RideShareDbContext> options)
        : base(options) { }

    public DbSet<Rider> Riders { get; set; } = null!;
    public DbSet<Driver> Drivers { get; set; } = null!;
    public DbSet<Ride> Rides { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ride>()
            .HasOne<Rider>()
            .WithMany()
            .HasForeignKey(r => r.RiderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ride>()
            .HasOne<Driver>()
            .WithMany()
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}