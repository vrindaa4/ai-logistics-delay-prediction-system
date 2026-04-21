using LogisticsAPI.Models;

namespace LogisticsAPI.Repositories;

public interface IAlertRepository
{
    Task<IEnumerable<Alert>> GetByShipmentIdAsync(int shipmentId);
    Task<Alert> CreateAsync(Alert alert);
    Task<Alert?> UpdateAsync(int id, Alert alert);
    Task<IEnumerable<Alert>> GetActiveAlertsAsync();
    Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(string severity);
    Task<IEnumerable<Alert>> GetAllAsync();
}
