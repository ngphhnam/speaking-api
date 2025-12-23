using SpeakingPractice.Api.DTOs.Payments;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Create a PayOS payment link for upgrading the current user to premium.
    /// </summary>
    Task<CreatePremiumPaymentResponse> CreatePremiumPaymentAsync(Guid userId, CreatePremiumPaymentRequest request, CancellationToken ct);

    /// <summary>
    /// Handle PayOS webhook callback and update user subscription if payment is successful.
    /// Returns the associated user id if a subscription was updated, otherwise null.
    /// </summary>
    Task<Guid?> HandlePayOsWebhookAsync(string rawBody, string signature, CancellationToken ct);
}


