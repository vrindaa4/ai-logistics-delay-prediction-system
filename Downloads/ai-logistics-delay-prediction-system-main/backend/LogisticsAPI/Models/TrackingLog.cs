namespace LogisticsAPI.Models;

public class TrackingLog
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public required string Location { get; set; }

    public required string Status { get; set; }

    public DateTime Timestamp { get; set; }
}
