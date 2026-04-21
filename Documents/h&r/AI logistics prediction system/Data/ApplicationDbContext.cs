using AI.Logistics.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AI.Logistics.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<TrackingUpdate> TrackingUpdates => Set<TrackingUpdate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>()
            .HasIndex(s => s.TrackingNumber)
            .IsUnique();

        modelBuilder.Entity<Shipment>()
            .Property(s => s.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TrackingUpdate>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Shipment>()
            .HasMany(s => s.TrackingUpdates)
            .WithOne(t => t.Shipment)
            .HasForeignKey(t => t.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
