using Microsoft.EntityFrameworkCore;
using LogisticsAPI.Models;

namespace LogisticsAPI.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    Console.WriteLine("Cannot connect to database. Attempting to create...");
                }

                await context.Database.MigrateAsync();
                
                await SeedDataAsync(context);
                
                Console.WriteLine(" Database initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                Console.WriteLine($"Make sure MySQL is running: brew services start mysql");
                Console.WriteLine($" Check connection string in appsettings.json");
                throw;
            }
        }
    }

   
    private static async Task SeedDataAsync(AppDbContext context)
    {
        if (await context.Shipments.AnyAsync())
        {
            Console.WriteLine("  Database already contains data. Skipping seed.");
            return;
        }

        try
        {
            var shipments = new List<Shipment>
            {
                new Shipment
                {
                    Origin = "New York",
                    Destination = "Los Angeles",
                    Carrier = "FedEx",
                    TrackingNumber = "FDX001",
                    EstimatedDeliveryDateUtc = DateTime.UtcNow.AddDays(5),
                    Status = "In Transit",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
                },
                new Shipment
                {
                    Origin = "Chicago",
                    Destination = "Miami",
                    Carrier = "UPS",
                    TrackingNumber = "UPS001",
                    EstimatedDeliveryDateUtc = DateTime.UtcNow.AddDays(3),
                    Status = "In Transit",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                },
                new Shipment
                {
                    Origin = "Seattle",
                    Destination = "Boston",
                    Carrier = "DHL",
                    TrackingNumber = "DHL001",
                    EstimatedDeliveryDateUtc = DateTime.UtcNow.AddDays(7),
                    Status = "Pending",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Shipment
                {
                    Origin = "San Francisco",
                    Destination = "New York",
                    Carrier = "USPS",
                    TrackingNumber = "USPS001",
                    EstimatedDeliveryDateUtc = DateTime.UtcNow.AddDays(4),
                    Status = "Delivered",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-5),
                    DeliveredAtUtc = DateTime.UtcNow.AddDays(-1)
                },
                new Shipment
                {
                    Origin = "Denver",
                    Destination = "Phoenix",
                    Carrier = "FedEx",
                    TrackingNumber = "FDX002",
                    EstimatedDeliveryDateUtc = DateTime.UtcNow.AddDays(-1),
                    Status = "Delayed",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
                }
            };

            await context.Shipments.AddRangeAsync(shipments);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {shipments.Count} shipments");

            var trackingLogs = new List<TrackingLog>();
            
            foreach (var shipment in shipments)
            {
                trackingLogs.Add(new TrackingLog
                {
                    ShipmentId = shipment.Id,
                    Location = shipment.Origin,
                    Status = "Picked up",
                    Timestamp = shipment.CreatedAtUtc
                });

                if (shipment.Status != "Pending")
                {
                    trackingLogs.Add(new TrackingLog
                    {
                        ShipmentId = shipment.Id,
                        Location = "Distribution Center",
                        Status = "In sorting",
                        Timestamp = shipment.CreatedAtUtc.AddHours(12)
                    });
                }

                if (shipment.Status == "Delivered" || shipment.Status == "In Transit")
                {
                    trackingLogs.Add(new TrackingLog
                    {
                        ShipmentId = shipment.Id,
                        Location = "Local delivery hub",
                        Status = "Out for delivery",
                        Timestamp = shipment.CreatedAtUtc.AddDays(2)
                    });
                }

                if (shipment.Status == "Delivered")
                {
                    trackingLogs.Add(new TrackingLog
                    {
                        ShipmentId = shipment.Id,
                        Location = shipment.Destination,
                        Status = "Delivered",
                        Timestamp = shipment.DeliveredAtUtc ?? DateTime.UtcNow
                    });
                }
            }

            await context.TrackingLogs.AddRangeAsync(trackingLogs);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {trackingLogs.Count} tracking logs");

            var alerts = new List<Alert>
            {
                new Alert
                {
                    ShipmentId = shipments[4].Id,
                    AlertType = "Delay",
                    Message = "Shipment is delayed by 2 days",
                    IsResolved = false,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                    Severity = "High"
                },
                new Alert
                {
                    ShipmentId = shipments[0].Id,
                    AlertType = "Weather",
                    Message = "Severe weather may impact delivery",
                    IsResolved = false,
                    CreatedAtUtc = DateTime.UtcNow,
                    Severity = "Medium"
                },
                new Alert
                {
                    ShipmentId = shipments[1].Id,
                    AlertType = "Resolved",
                    Message = "Previously delayed shipment is back on track",
                    IsResolved = true,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
                    ResolvedAtUtc = DateTime.UtcNow.AddDays(-1),
                    Severity = "Low"
                }
            };

            await context.Alerts.AddRangeAsync(alerts);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {alerts.Count} alerts");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
            throw;
        }
    }
}
