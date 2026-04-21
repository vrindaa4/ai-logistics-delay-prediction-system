using Microsoft.EntityFrameworkCore;
using LogisticsAPI.Models;

namespace LogisticsAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<TrackingLog> TrackingLogs { get; set; }
    public DbSet<Alert> Alerts { get; set; }
}
