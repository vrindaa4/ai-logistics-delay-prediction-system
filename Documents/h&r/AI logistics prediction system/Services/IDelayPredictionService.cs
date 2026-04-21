using AI.Logistics.Api.Models;

namespace AI.Logistics.Api.Services;

public interface IDelayPredictionService
{
    (bool IsLikelyDelayed, double Confidence) Predict(Shipment shipment, int updatesCount);
}
