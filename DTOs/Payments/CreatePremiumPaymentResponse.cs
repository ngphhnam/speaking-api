namespace SpeakingPractice.Api.DTOs.Payments;

public class CreatePremiumPaymentResponse
{
    /// <summary>
    /// URL that frontend should redirect the user to complete the payment.
    /// </summary>
    public string CheckoutUrl { get; set; } = string.Empty;

    /// <summary>
    /// PayOS order code / reference for tracking.
    /// </summary>
    public string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional raw payment link id or code returned by PayOS.
    /// </summary>
    public string? PaymentId { get; set; }

    /// <summary>
    /// PayOS paymentLinkId if provided (useful for generating QR).
    /// </summary>
    public string? PaymentLinkId { get; set; }

    /// <summary>
    /// Raw QR string (qrCode) returned directly by PayOS, if available.
    /// </summary>
    public string? QrCode { get; set; }

    /// <summary>
    /// Pre-built QR image URL (VietQR) if bin/accountNumber/accountName are present.
    /// </summary>
    public string? QrImageUrl { get; set; }
}


