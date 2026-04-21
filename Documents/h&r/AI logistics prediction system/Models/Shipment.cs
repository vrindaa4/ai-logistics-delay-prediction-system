using System.ComponentModel.DataAnnotations;

namespace AI.Logistics.Api.Models;

public class Shipment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(120)]
    public string Origin { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string Carrier { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string TrackingNumber { get; set; } = string.Empty;

    public DateTime EstimatedDeliveryDateUtc { get; set; }
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAtUtc { get; set; }

    public ICollection<TrackingUpdate> TrackingUpdates { get; set; } = new List<TrackingUpdate>();
}
