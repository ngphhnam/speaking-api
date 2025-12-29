namespace SpeakingPractice.Api.Options;

public class PayOsOptions
{
    public const string SectionName = "PayOS";

    /// <summary>
    /// Base URL of the PayOS API, e.g. https://api.payos.vn
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Client ID provided by PayOS.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// API key provided by PayOS.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Checksum / signature key for verifying callbacks.
    /// </summary>
    public string ChecksumKey { get; set; } = string.Empty;

    /// <summary>
    /// Default return URL after successful payment.
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// Default cancel URL when user aborts payment.
    /// </summary>
    public string CancelUrl { get; set; } = string.Empty;

    /// <summary>
    /// Webhook URL path configured in PayOS for server-to-server callbacks.
    /// </summary>
    public string WebhookPath { get; set; } = "/api/payments/webhook";

    /// <summary>
    /// Number of days to extend the premium subscription on successful payment.
    /// </summary>
    public int PremiumDays { get; set; } = 30;

    /// <summary>
    /// Subscription type string to set for premium users.
    /// </summary>
    public string PremiumSubscriptionType { get; set; } = "premium";
}








