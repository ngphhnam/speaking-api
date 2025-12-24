namespace SpeakingPractice.Api.DTOs.Payments;

public class CreatePremiumPaymentRequest
{
    /// <summary>
    /// Optional client-side idempotency/order reference. If not provided, the API will generate one.
    /// </summary>
    public string? ClientReference { get; set; }

    /// <summary>
    /// Optional amount override. If not provided, backend can use a default premium price.
    /// </summary>
    public long? Amount { get; set; }

    /// <summary>
    /// Premium plan code: premium_1m, premium_3m, premium_6m, premium_12m. Defaults to premium_1m.
    /// </summary>
    public string? PlanCode { get; set; }
}


