using Microsoft.AspNetCore.Mvc;
using LogisticsAPI.Models;
using LogisticsAPI.Repositories;
using LogisticsAPI.Services;
using LogisticsAPI.DTOs;

namespace LogisticsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ITrackingLogRepository _trackingLogRepository;
    private readonly PredictionService _predictionService;
    private readonly AlertService _alertService;

    public ShipmentController(
        IShipmentRepository shipmentRepository,
        ITrackingLogRepository trackingLogRepository,
        PredictionService predictionService,
        AlertService alertService)
    {
        _shipmentRepository = shipmentRepository;
        _trackingLogRepository = trackingLogRepository;
        _predictionService = predictionService;
        _alertService = alertService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var shipment = new Shipment
        {
            Origin = dto.Origin,
            Destination = dto.Destination,
            Carrier = dto.Carrier,
            TrackingNumber = dto.TrackingNumber,
            EstimatedDeliveryDateUtc = dto.EstimatedDeliveryDateUtc,
            Status = dto.Status
        };

        var created = await _shipmentRepository.CreateAsync(shipment);
        return CreatedAtAction(nameof(GetShipment), new { id = created.Id }, MapToDto(created));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShipments()
    {
        var shipments = await _shipmentRepository.GetAllAsync();
        return Ok(shipments.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipment(int id)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        if (shipment == null)
            return NotFound();

        return Ok(MapToDto(shipment));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShipment(int id, [FromBody] Shipment shipment)
    {
        var updated = await _shipmentRepository.UpdateAsync(id, shipment);
        if (updated == null)
            return NotFound();

        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShipment(int id)
    {
        var deleted = await _shipmentRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchShipments([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest();

        var shipments = await _shipmentRepository.SearchAsync(query);
        return Ok(shipments.Select(MapToDto));
    }

    [HttpGet("filter/status")]
    public async Task<IActionResult> FilterByStatus([FromQuery] string status)
    {
        var shipments = await _shipmentRepository.FilterByStatusAsync(status);
        return Ok(shipments.Select(MapToDto));
    }

    [HttpGet("filter/carrier")]
    public async Task<IActionResult> FilterByCarrier([FromQuery] string carrier)
    {
        var shipments = await _shipmentRepository.FilterByCarrierAsync(carrier);
        return Ok(shipments.Select(MapToDto));
    }

    [HttpGet("delayed")]
    public async Task<IActionResult> GetDelayedShipments()
    {
        var shipments = await _shipmentRepository.GetDelayedShipmentsAsync();
        return Ok(shipments.Select(MapToDto));
    }

    [HttpGet("{id}/predict")]
    public async Task<IActionResult> PredictDelay(int id)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        if (shipment == null)
            return NotFound();

        var trackingLogs = await _trackingLogRepository.GetByShipmentIdAsync(id);
        var prediction = _predictionService.PredictDelay(shipment, trackingLogs.Count());

        if (prediction.IsDelayed)
        {
            await _alertService.CreateDelayAlertAsync(id, prediction.Recommendation, prediction.RiskLevel);
        }

        return Ok(prediction);
    }

    private static ShipmentResponseDto MapToDto(Shipment shipment)
    {
        return new ShipmentResponseDto
        {
            Id = shipment.Id,
            Origin = shipment.Origin,
            Destination = shipment.Destination,
            Carrier = shipment.Carrier,
            TrackingNumber = shipment.TrackingNumber,
            EstimatedDeliveryDateUtc = shipment.EstimatedDeliveryDateUtc,
            Status = shipment.Status,
            CreatedAtUtc = shipment.CreatedAtUtc,
            DeliveredAtUtc = shipment.DeliveredAtUtc
        };
    }
}
