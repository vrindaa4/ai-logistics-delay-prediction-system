namespace LogisticsAPI.DTOs;

public class ErrorResponseDto
{
    public int StatusCode { get; set; }
    public required string Message { get; set; }
    public required string ErrorCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}
