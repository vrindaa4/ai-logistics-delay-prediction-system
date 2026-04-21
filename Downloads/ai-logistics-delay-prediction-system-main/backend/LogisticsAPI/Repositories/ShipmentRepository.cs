using LogisticsAPI.Data;
using LogisticsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsAPI.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly AppDbContext _context;

    public ShipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Shipment>> GetAllAsync()
    {
        return await _context.Shipments.OrderByDescending(s => s.CreatedAtUtc).ToListAsync();
    }

    public async Task<Shipment?> GetByIdAsync(int id)
    {
        return await _context.Shipments.FindAsync(id);
    }

    public async Task<Shipment> CreateAsync(Shipment shipment)
    {
        shipment.CreatedAtUtc = DateTime.UtcNow;
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();
        return shipment;
    }

    public async Task<Shipment?> UpdateAsync(int id, Shipment shipment)
    {
        var existingShipment = await _context.Shipments.FindAsync(id);
        if (existingShipment == null)
            return null;

        existingShipment.Status = shipment.Status;
        existingShipment.EstimatedDeliveryDateUtc = shipment.EstimatedDeliveryDateUtc;
        existingShipment.DeliveredAtUtc = shipment.DeliveredAtUtc;

        _context.Shipments.Update(existingShipment);
        await _context.SaveChangesAsync();
        return existingShipment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment == null)
            return false;

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Shipment>> SearchAsync(string searchTerm)
    {
        return await _context.Shipments
            .Where(s => s.TrackingNumber.Contains(searchTerm) ||
                        s.Origin.Contains(searchTerm) ||
                        s.Destination.Contains(searchTerm) ||
                        s.Carrier.Contains(searchTerm))
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> FilterByStatusAsync(string status)
    {
        return await _context.Shipments
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> FilterByCarrierAsync(string carrier)
    {
        return await _context.Shipments
            .Where(s => s.Carrier == carrier)
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetDelayedShipmentsAsync()
    {
        return await _context.Shipments
            .Where(s => DateTime.UtcNow > s.EstimatedDeliveryDateUtc && s.Status != "Delivered")
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync();
    }
}
