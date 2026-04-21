using LogisticsAPI.Data;
using LogisticsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsAPI.Repositories;

public class TrackingLogRepository : ITrackingLogRepository
{
    private readonly AppDbContext _context;

    public TrackingLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrackingLog>> GetByShipmentIdAsync(int shipmentId)
    {
        return await _context.TrackingLogs
            .Where(t => t.ShipmentId == shipmentId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }

    public async Task<TrackingLog> CreateAsync(TrackingLog log)
    {
        log.Timestamp = DateTime.UtcNow;
        _context.TrackingLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<TrackingLog>> GetAllAsync()
    {
        return await _context.TrackingLogs
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }
}
