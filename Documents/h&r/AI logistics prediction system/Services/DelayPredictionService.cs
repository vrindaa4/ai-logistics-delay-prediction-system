using AI.Logistics.Api.Models;

namespace AI.Logistics.Api.Services;

public class DelayPredictionService : IDelayPredictionService
{
    public (bool IsLikelyDelayed, double Confidence) Predict(Shipment shipment, int updatesCount)
    {
        if (shipment.Status == ShipmentStatus.Delivered)
        {
            return (false, 0.98);
        }

        var daysToEta = (shipment.EstimatedDeliveryDateUtc - DateTime.UtcNow).TotalDays;
        var overdue = daysToEta < 0;
        var sparseUpdates = updatesCount < 2;
        var explicitDelay = shipment.Status == ShipmentStatus.Delayed;

        var score = 0.15;
        if (overdue) score += 0.45;
        if (sparseUpdates) score += 0.20;
        if (explicitDelay) score += 0.30;

        var confidence = Math.Clamp(score, 0.0, 0.99);
        return (confidence >= 0.60, confidence);
    }
}
