namespace LogisticsAPI.DTOs;

public class AlertResponseDto
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public required string AlertType { get; set; }
    public required string Message { get; set; }
    public bool IsResolved { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public required string Severity { get; set; }
}

public class PredictionResponseDto
{
    public int ShipmentId { get; set; }
    public bool IsDelayed { get; set; }
    public int DelayScore { get; set; }
    public required string RiskLevel { get; set; }
    public required string Recommendation { get; set; }
}
