using AI.Logistics.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace AI.Logistics.Api.Dtos;

public class AddTrackingUpdateRequest
{
    [Required]
    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Remarks { get; set; } = string.Empty;

    [Required]
    public ShipmentStatus Status { get; set; }

    public DateTime? TimestampUtc { get; set; }
}
