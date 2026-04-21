using LogisticsAPI.Models;

namespace LogisticsAPI.Repositories;

public interface ITrackingLogRepository
{
    Task<IEnumerable<TrackingLog>> GetByShipmentIdAsync(int shipmentId);
    Task<TrackingLog> CreateAsync(TrackingLog log);
    Task<IEnumerable<TrackingLog>> GetAllAsync();
}
