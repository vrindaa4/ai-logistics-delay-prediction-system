namespace LogisticsAPI.DTOs;

public class DashboardStatsDto
{
    public int TotalShipments { get; set; }
    public int DelayedShipments { get; set; }
    public int OnTimeShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public double AverageDelayPercentage { get; set; }
    public int ActiveAlerts { get; set; }
    public int CriticalAlerts { get; set; }
}

public class ShipmentMetricsDto
{
    public required string CarrierName { get; set; }
    public int TotalShipments { get; set; }
    public int DelayedCount { get; set; }
    public double DelayPercentage { get; set; }
}
