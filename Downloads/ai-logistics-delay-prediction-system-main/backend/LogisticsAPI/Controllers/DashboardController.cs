using Microsoft.AspNetCore.Mvc;
using LogisticsAPI.Services;
using LogisticsAPI.DTOs;

namespace LogisticsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _dashboardService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    [HttpGet("metrics/carriers")]
    [ProducesResponseType(typeof(IEnumerable<ShipmentMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarrierMetrics()
    {
        var metrics = await _dashboardService.GetCarrierMetricsAsync();
        return Ok(metrics);
    }

    [HttpGet("summary/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusSummary()
    {
        var (inTransit, pending, delivered) = await _dashboardService.GetStatusSummaryAsync();

        return Ok(new
        {
            inTransit,
            pending,
            delivered,
            total = inTransit + pending + delivered
        });
    }
}
