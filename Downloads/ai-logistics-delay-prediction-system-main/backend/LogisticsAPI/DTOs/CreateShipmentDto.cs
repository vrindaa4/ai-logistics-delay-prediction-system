using System.ComponentModel.DataAnnotations;

namespace LogisticsAPI.DTOs;

public class CreateShipmentDto
{
    [Required(ErrorMessage = "Origin is required")]
    [StringLength(100, ErrorMessage = "Origin must be less than 100 characters")]
    public required string Origin { get; set; }

    [Required(ErrorMessage = "Destination is required")]
    [StringLength(100, ErrorMessage = "Destination must be less than 100 characters")]
    public required string Destination { get; set; }

    [Required(ErrorMessage = "Carrier is required")]
    [StringLength(50, ErrorMessage = "Carrier must be less than 50 characters")]
    public required string Carrier { get; set; }

    [Required(ErrorMessage = "Tracking number is required")]
    [StringLength(50, ErrorMessage = "Tracking number must be less than 50 characters")]
    public required string TrackingNumber { get; set; }

    [Required(ErrorMessage = "Estimated delivery date is required")]
    [DataType(DataType.DateTime)]
    public DateTime EstimatedDeliveryDateUtc { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(20, ErrorMessage = "Status must be less than 20 characters")]
    public required string Status { get; set; }
}
