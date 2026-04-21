using LogisticsAPI.Models;
using LogisticsAPI.Repositories;

namespace LogisticsAPI.Services;

public class AlertService
{
    private readonly IAlertRepository _alertRepository;

    public AlertService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<Alert> CreateDelayAlertAsync(int shipmentId, string message, string riskLevel)
    {
        var alert = new Alert
        {
            ShipmentId = shipmentId,
            AlertType = "DelayWarning",
            Message = message,
            Severity = riskLevel,
            IsResolved = false
        };

        return await _alertRepository.CreateAsync(alert);
    }

    public async Task<Alert> CreateSystemAlertAsync(int shipmentId, string alertType, string message)
    {
        var alert = new Alert
        {
            ShipmentId = shipmentId,
            AlertType = alertType,
            Message = message,
            Severity = "Medium",
            IsResolved = false
        };

        return await _alertRepository.CreateAsync(alert);
    }

    public async Task<Alert?> ResolveAlertAsync(int alertId)
    {
        var alert = new Alert 
        { 
            IsResolved = true,
            AlertType = "",
            Message = "",
            Severity = ""
        };
        return await _alertRepository.UpdateAsync(alertId, alert);
    }

    public async Task<int> GetCriticalAlertCountAsync()
    {
        var alerts = await _alertRepository.GetAlertsBySeverityAsync("Critical");
        return alerts.Count();
    }

    public async Task<int> GetActiveAlertCountAsync()
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync();
        return alerts.Count();
    }
}
