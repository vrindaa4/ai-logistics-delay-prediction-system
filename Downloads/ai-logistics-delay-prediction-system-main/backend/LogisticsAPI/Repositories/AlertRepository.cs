using LogisticsAPI.Data;
using LogisticsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsAPI.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;

    public AlertRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Alert>> GetByShipmentIdAsync(int shipmentId)
    {
        return await _context.Alerts
            .Where(a => a.ShipmentId == shipmentId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Alert> CreateAsync(Alert alert)
    {
        alert.CreatedAtUtc = DateTime.UtcNow;
        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();
        return alert;
    }

    public async Task<Alert?> UpdateAsync(int id, Alert alert)
    {
        var existingAlert = await _context.Alerts.FindAsync(id);
        if (existingAlert == null)
            return null;

        existingAlert.IsResolved = alert.IsResolved;
        existingAlert.ResolvedAtUtc = alert.IsResolved ? DateTime.UtcNow : null;

        _context.Alerts.Update(existingAlert);
        await _context.SaveChangesAsync();
        return existingAlert;
    }

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync()
    {
        return await _context.Alerts
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(string severity)
    {
        return await _context.Alerts
            .Where(a => a.Severity == severity && !a.IsResolved)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> GetAllAsync()
    {
        return await _context.Alerts
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync();
    }
}
