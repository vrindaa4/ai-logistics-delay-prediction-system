using System.ComponentModel.DataAnnotations;

namespace AI.Logistics.Api.Dtos;

public class CreateShipmentRequest
{
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
}
