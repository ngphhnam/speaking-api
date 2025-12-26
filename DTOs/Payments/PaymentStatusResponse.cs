namespace SpeakingPractice.Api.DTOs.Payments;

public class PaymentStatusResponse
{
    public string OrderCode { get; set; } = string.Empty;
    public string? PaymentLinkId { get; set; }
    public string Status { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string Provider { get; set; } = string.Empty;
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? QrImageUrl { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}







