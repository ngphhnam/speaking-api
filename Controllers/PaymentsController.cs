using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.Payments;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentsController(
    IPaymentService paymentService,
    ILogger<PaymentsController> logger) : ControllerBase
{
    [HttpPost("premium/checkout")]
    [Authorize]
    public async Task<IActionResult> CreatePremiumCheckout(
        [FromBody] CreatePremiumPaymentRequest request,
        CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "Invalid user id");
        }

        var response = await paymentService.CreatePremiumPaymentAsync(userId, request, ct);
        return this.ApiOk(response, "Payment link created successfully");
    }

    /// <summary>
    /// Webhook endpoint for PayOS to notify payment status.
    /// Configure this URL in PayOS dashboard to match PayOsOptions.WebhookPath.
    /// </summary>
    [HttpPost("payos-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PayOsWebhook(CancellationToken ct)
    {
        string rawBody;
        using (var reader = new StreamReader(Request.Body))
        {
            rawBody = await reader.ReadToEndAsync();
        }

        // Lấy signature từ các header phổ biến mà PayOS có thể gửi
        var signature =
            Request.Headers["x-signature"].ToString()
            ?? Request.Headers["x-payos-signature"].ToString()
            ?? Request.Headers["x-webhook-signature"].ToString();

        // Logging để debug webhook từ PayOS
        logger.LogInformation("Received PayOS webhook. Signature header: {Signature}", signature);
        logger.LogInformation("PayOS webhook raw body: {Body}", rawBody);

        var userId = await paymentService.HandlePayOsWebhookAsync(rawBody, signature, ct);
        if (userId is null)
        {
            // Always return 200 to acknowledge webhook even if we didn't process it,
            // to avoid PayOS retry storms. Logging is handled in the service.
            return Ok();
        }

        logger.LogInformation("Processed PayOS webhook and upgraded user {UserId}", userId.Value);
        return Ok();
    }

    /// <summary>
    /// FE can poll this endpoint with orderCode or paymentLinkId to know current payment status.
    /// </summary>
    [HttpGet("status/{code}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentStatus([FromRoute] string code, CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var status = await paymentService.GetPaymentStatusAsync(userId, code, ct);
        if (status is null)
        {
            return this.ApiNotFound("Payment not found");
        }

        return this.ApiOk(status);
    }
}


