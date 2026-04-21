using System.ComponentModel.DataAnnotations;

namespace AI.Logistics.Api.Models;

public class TrackingUpdate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShipmentId { get; set; }

    [Required]
    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Remarks { get; set; } = string.Empty;

    public ShipmentStatus Status { get; set; } = ShipmentStatus.InTransit;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public Shipment? Shipment { get; set; }
}
