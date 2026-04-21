using AI.Logistics.Api.Data;
using AI.Logistics.Api.Dtos;
using AI.Logistics.Api.Models;
using AI.Logistics.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AI.Logistics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController(ApplicationDbContext db, IDelayPredictionService delayPredictionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        var alreadyExists = await db.Shipments.AnyAsync(s => s.TrackingNumber == request.TrackingNumber);
        if (alreadyExists)
        {
            return Conflict(new { message = "A shipment with this tracking number already exists." });
        }

        var shipment = new Shipment
        {
            Origin = request.Origin,
            Destination = request.Destination,
            Carrier = request.Carrier,
            TrackingNumber = request.TrackingNumber,
            EstimatedDeliveryDateUtc = request.EstimatedDeliveryDateUtc,
            Status = ShipmentStatus.Created
        };

        db.Shipments.Add(shipment);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetShipmentDetails), new { id = shipment.Id }, shipment);
    }

    [HttpPost("{id:guid}/tracking")]
    public async Task<IActionResult> AddTrackingUpdate(Guid id, [FromBody] AddTrackingUpdateRequest request)
    {
        var shipment = await db.Shipments.FirstOrDefaultAsync(s => s.Id == id);
        if (shipment is null)
        {
            return NotFound(new { message = "Shipment not found." });
        }

        var update = new TrackingUpdate
        {
            ShipmentId = shipment.Id,
            Location = request.Location,
            Remarks = request.Remarks,
            Status = request.Status,
            TimestampUtc = request.TimestampUtc ?? DateTime.UtcNow
        };

        shipment.Status = request.Status;
        if (request.Status == ShipmentStatus.Delivered)
        {
            shipment.DeliveredAtUtc = update.TimestampUtc;
        }

        db.TrackingUpdates.Add(update);
        await db.SaveChangesAsync();

        return Ok(update);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetShipmentDetails(Guid id)
    {
        var shipment = await db.Shipments
            .Include(s => s.TrackingUpdates.OrderByDescending(t => t.TimestampUtc))
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shipment is null)
        {
            return NotFound(new { message = "Shipment not found." });
        }

        var prediction = delayPredictionService.Predict(shipment, shipment.TrackingUpdates.Count);

        return Ok(new
        {
            shipment.Id,
            shipment.Origin,
            shipment.Destination,
            shipment.Carrier,
            shipment.TrackingNumber,
            shipment.CreatedAtUtc,
            shipment.EstimatedDeliveryDateUtc,
            shipment.DeliveredAtUtc,
            shipment.Status,
            DelayPrediction = new
            {
                prediction.IsLikelyDelayed,
                prediction.Confidence
            },
            TrackingUpdates = shipment.TrackingUpdates
                .OrderByDescending(t => t.TimestampUtc)
                .Select(t => new
                {
                    t.Id,
                    t.Location,
                    t.Remarks,
                    t.Status,
                    t.TimestampUtc
                })
        });
    }
}
