using LogisticsAPI.Models;
using LogisticsAPI.DTOs;

namespace LogisticsAPI.Services;

public class PredictionService
{
    public PredictionResponseDto PredictDelay(Shipment shipment, int trackingCount)
    {
        int delayScore = 0;
        var now = DateTime.UtcNow;
        var daysUntilDelivery = (shipment.EstimatedDeliveryDateUtc - now).TotalDays;

        if (shipment.Status == "InTransit")
            delayScore += 2;

        if (now > shipment.EstimatedDeliveryDateUtc && shipment.Status != "Delivered")
            delayScore += 5;

        if (daysUntilDelivery < 1 && shipment.Status != "Delivered")
            delayScore += 3;

        if (trackingCount > 5)
            delayScore += 2;

        if (trackingCount == 0)
            delayScore += 1;

        string riskLevel = delayScore switch
        {
            >= 9 => "Critical",
            >= 7 => "High",
            >= 5 => "Medium",
            >= 3 => "Low",
            _ => "Minimal"
        };

        bool isDelayed = delayScore >= 5;

        string recommendation = delayScore switch
        {
            >= 9 => "Immediate action required. Contact carrier and recipient immediately.",
            >= 7 => "High risk of delay. Monitor closely and prepare contingency.",
            >= 5 => "Potential delay detected. Follow up with carrier.",
            >= 3 => "Low risk. Continue normal monitoring.",
            _ => "Shipment on track."
        };

        return new PredictionResponseDto
        {
            ShipmentId = shipment.Id,
            IsDelayed = isDelayed,
            DelayScore = delayScore,
            RiskLevel = riskLevel,
            Recommendation = recommendation
        };
    }
}