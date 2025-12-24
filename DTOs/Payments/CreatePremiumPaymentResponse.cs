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

    /// <summary>
    /// Expiration time (UTC) of the PayOS payment link.
    /// </summary>
    public DateTimeOffset ExpiredAt { get; set; }

    /// <summary>
    /// Bank account name returned by PayOS (if available).
    /// </summary>
    public string? BankAccountName { get; set; }

    /// <summary>
    /// Bank account number returned by PayOS (if available).
    /// </summary>
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// Description sent to PayOS (for display on FE).
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Plan code used to create this payment (premium_1m/premium_3m/premium_6m/premium_12m).
    /// </summary>
    public string PlanCode { get; set; } = "premium_1m";

    /// <summary>
    /// Plan duration in days.
    /// </summary>
    public int PlanDays { get; set; } = 30;

    /// <summary>
    /// Plan price (amount) in smallest currency unit (e.g., VND).
    /// </summary>
    public long PlanPrice { get; set; }
}


