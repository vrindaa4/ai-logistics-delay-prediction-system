namespace LogisticsAPI.Models;

public class Alert
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public required string AlertType { get; set; }

    public required string Message { get; set; }

    public bool IsResolved { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ResolvedAtUtc { get; set; }

    public required string Severity { get; set; } // Critical, High, Medium, Low
}
