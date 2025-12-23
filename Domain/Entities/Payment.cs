using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;

    public string OrderCode { get; set; } = string.Empty;
    public string? ClientReference { get; set; }
    public string? PaymentLinkId { get; set; }
    public string? PaymentId { get; set; }

    public long Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "pending";
    public string Provider { get; set; } = "PayOS";
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? QrImageUrl { get; set; }

    public DateTimeOffset? ExpiredAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Last raw response (JSON) from provider, for audit/debug.
    /// </summary>
    public string? ProviderResponse { get; set; }
}

