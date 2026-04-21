using System.ComponentModel.DataAnnotations;

namespace LogisticsAPI.DTOs;

public class CreateTrackingLogDto
{
    [Required(ErrorMessage = "Shipment ID is required")]
    public int ShipmentId { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location must be less than 100 characters")]
    public required string Location { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status must be less than 50 characters")]
    public required string Status { get; set; }
}

public class TrackingLogResponseDto
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public required string Location { get; set; }
    public required string Status { get; set; }
    public DateTime Timestamp { get; set; }
}
