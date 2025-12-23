namespace SpeakingPractice.Api.DTOs.Payments;

/// <summary>
/// Minimal DTO for handling PayOS webhook callbacks.
/// Shape may be adjusted to exactly match PayOS payload.
/// </summary>
public class PayOsWebhookDto
{
    public string OrderCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Signature { get; set; } = string.Empty;
    public string RawBody { get; set; } = string.Empty;
}


