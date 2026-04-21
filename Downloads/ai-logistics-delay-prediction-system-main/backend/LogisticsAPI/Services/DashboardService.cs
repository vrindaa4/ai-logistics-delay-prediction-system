using LogisticsAPI.DTOs;
using LogisticsAPI.Models;
using LogisticsAPI.Repositories;

namespace LogisticsAPI.Services;

public class DashboardService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IAlertRepository _alertRepository;

    public DashboardService(IShipmentRepository shipmentRepository, IAlertRepository alertRepository)
    {
        _shipmentRepository = shipmentRepository;
        _alertRepository = alertRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var allShipments = await _shipmentRepository.GetAllAsync();
        var delayedShipments = await _shipmentRepository.GetDelayedShipmentsAsync();
        var activeAlerts = await _alertRepository.GetActiveAlertsAsync();
        var criticalAlerts = await _alertRepository.GetAlertsBySeverityAsync("Critical");

        var totalShipments = allShipments.Count();
        var delayedCount = delayedShipments.Count();
        var deliveredCount = allShipments.Count(s => s.Status == "Delivered");
        var onTimeCount = totalShipments - delayedCount;

        double delayPercentage = totalShipments > 0 ? (double)delayedCount / totalShipments * 100 : 0;

        return new DashboardStatsDto
        {
            TotalShipments = totalShipments,
            DelayedShipments = delayedCount,
            OnTimeShipments = onTimeCount,
            DeliveredShipments = deliveredCount,
            AverageDelayPercentage = Math.Round(delayPercentage, 2),
            ActiveAlerts = activeAlerts.Count(),
            CriticalAlerts = criticalAlerts.Count()
        };
    }

    public async Task<IEnumerable<ShipmentMetricsDto>> GetCarrierMetricsAsync()
    {
        var allShipments = await _shipmentRepository.GetAllAsync();
        var delayedShipments = await _shipmentRepository.GetDelayedShipmentsAsync();

        var metrics = allShipments
            .GroupBy(s => s.Carrier)
            .Select(g => new ShipmentMetricsDto
            {
                CarrierName = g.Key,
                TotalShipments = g.Count(),
                DelayedCount = delayedShipments.Count(s => s.Carrier == g.Key),
                DelayPercentage = Math.Round((double)delayedShipments.Count(s => s.Carrier == g.Key) / g.Count() * 100, 2)
            })
            .OrderByDescending(m => m.DelayPercentage);

        return metrics;
    }

    public async Task<(int, int, int)> GetStatusSummaryAsync()
    {
        var allShipments = await _shipmentRepository.GetAllAsync();

        int inTransit = allShipments.Count(s => s.Status == "InTransit");
        int pending = allShipments.Count(s => s.Status == "Pending");
        int delivered = allShipments.Count(s => s.Status == "Delivered");

        return (inTransit, pending, delivered);
    }
}
