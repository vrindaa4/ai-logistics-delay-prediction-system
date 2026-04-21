using Microsoft.AspNetCore.Mvc;
using LogisticsAPI.Repositories;
using LogisticsAPI.DTOs;
using LogisticsAPI.Models;

namespace LogisticsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertController : ControllerBase
{
    private readonly IAlertRepository _alertRepository;

    public AlertController(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAlerts()
    {
        var alerts = await _alertRepository.GetAllAsync();
        var response = alerts.Select(MapToResponseDto).ToList();
        return Ok(response);
    }

    [HttpGet("shipment/{shipmentId}")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertsByShipment(int shipmentId)
    {
        var alerts = await _alertRepository.GetByShipmentIdAsync(shipmentId);
        var response = alerts.Select(MapToResponseDto).ToList();
        return Ok(response);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveAlerts()
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync();
        var response = alerts.Select(MapToResponseDto).ToList();
        return Ok(response);
    }

    [HttpGet("severity/{severity}")]
    [ProducesResponseType(typeof(IEnumerable<AlertResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertsBySeverity(string severity)
    {
        var validSeverities = new[] { "Critical", "High", "Medium", "Low" };
        if (!validSeverities.Contains(severity))
            return BadRequest(new ErrorResponseDto
            {
                StatusCode = 400,
                Message = $"Invalid severity. Allowed values: {string.Join(", ", validSeverities)}",
                ErrorCode = "INVALID_SEVERITY",
                Timestamp = DateTime.UtcNow
            });

        var alerts = await _alertRepository.GetAlertsBySeverityAsync(severity);
        var response = alerts.Select(MapToResponseDto).ToList();
        return Ok(response);
    }

    [HttpPut("{id}/resolve")]
    [ProducesResponseType(typeof(AlertResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveAlert(int id)
    {
        var alert = new Alert 
        { 
            IsResolved = true,
            AlertType = "",
            Message = "",
            Severity = ""
        };
        var updated = await _alertRepository.UpdateAsync(id, alert);

        if (updated == null)
            return NotFound(new ErrorResponseDto
            {
                StatusCode = 404,
                Message = $"Alert with ID {id} not found",
                ErrorCode = "ALERT_NOT_FOUND",
                Timestamp = DateTime.UtcNow
            });

        return Ok(MapToResponseDto(updated));
    }

    private static AlertResponseDto MapToResponseDto(Alert alert)
    {
        return new AlertResponseDto
        {
            Id = alert.Id,
            ShipmentId = alert.ShipmentId,
            AlertType = alert.AlertType,
            Message = alert.Message,
            IsResolved = alert.IsResolved,
            CreatedAtUtc = alert.CreatedAtUtc,
            ResolvedAtUtc = alert.ResolvedAtUtc,
            Severity = alert.Severity
        };
    }
}
