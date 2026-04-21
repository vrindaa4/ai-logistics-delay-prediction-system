using Microsoft.AspNetCore.Mvc;
using LogisticsAPI.Models;
using LogisticsAPI.Repositories;
using LogisticsAPI.DTOs;

namespace LogisticsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackingController : ControllerBase
{
    private readonly ITrackingLogRepository _trackingLogRepository;

    public TrackingController(ITrackingLogRepository trackingLogRepository)
    {
        _trackingLogRepository = trackingLogRepository;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TrackingLogResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrackingLog([FromBody] CreateTrackingLogDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var log = new TrackingLog
        {
            ShipmentId = dto.ShipmentId,
            Location = dto.Location,
            Status = dto.Status
        };

        var created = await _trackingLogRepository.CreateAsync(log);

        var response = MapToResponseDto(created);
        return CreatedAtAction(nameof(GetTrackingLogsByShipment), new { shipmentId = created.ShipmentId }, response);
    }

    [HttpGet("shipment/{shipmentId}")]
    [ProducesResponseType(typeof(IEnumerable<TrackingLogResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrackingLogsByShipment(int shipmentId)
    {
        var logs = await _trackingLogRepository.GetByShipmentIdAsync(shipmentId);
        var response = logs.Select(MapToResponseDto).ToList();
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TrackingLogResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTrackingLogs()
    {
        var logs = await _trackingLogRepository.GetAllAsync();
        var response = logs.Select(MapToResponseDto).ToList();
        return Ok(response);
    }

    private static TrackingLogResponseDto MapToResponseDto(TrackingLog log)
    {
        return new TrackingLogResponseDto
        {
            Id = log.Id,
            ShipmentId = log.ShipmentId,
            Location = log.Location,
            Status = log.Status,
            Timestamp = log.Timestamp
        };
    }
}
