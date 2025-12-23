using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Payments;
using SpeakingPractice.Api.Options;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class PaymentService(
    IHttpClientFactory httpClientFactory,
    IOptions<PayOsOptions> payOsOptions,
    UserManager<ApplicationUser> userManager,
    ILogger<PaymentService> logger) : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly PayOsOptions _options = payOsOptions.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<PaymentService> _logger = logger;

    public async Task<CreatePremiumPaymentResponse> CreatePremiumPaymentAsync(
        Guid userId,
        CreatePremiumPaymentRequest request,
        CancellationToken ct)
    {
        // PayOS yêu cầu orderCode là số nguyên dương, <= 9007199254740991
        // -> dùng timestamp milliseconds để vừa duy nhất vừa đúng constraint.
        // (Nếu sau này cần map userId <-> orderCode, ta lưu mapping trong DB thay vì nhét vào chuỗi orderCode)
        long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Default amount if not provided – you can adjust this to your real premium price
        var amount = request.Amount ?? 10000; // example: 100,000 VND

        // PayOS giới hạn description tối đa 25 ký tự
        var description = "Premium Upgrade";
        var nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expiredAt = nowUnix + 3600; // 1 hour

        // Tạo chuỗi raw để ký giống với Postman (encodeUri: false)
        // amount, cancelUrl, description, orderCode, returnUrl
        var rawSignatureString =
            $"amount={amount}" +
            $"&cancelUrl={_options.CancelUrl}" +
            $"&description={description}" +
            $"&orderCode={orderCode.ToString()}" +
            $"&returnUrl={_options.ReturnUrl}";

        string signature;
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.ChecksumKey)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawSignatureString));
            signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        var body = new
        {
            orderCode,
            amount,
            description,
            returnUrl = _options.ReturnUrl,
            cancelUrl = _options.CancelUrl,
            expiredAt,
            signature
        };

        using var client = _httpClientFactory.CreateClient("PayOS");

        HttpResponseMessage response;
        try
        {
            // NOTE: Adjust the endpoint and payload to match the official PayOS REST API.
            // This is a reasonable default; consult PayOS docs for the exact path.
            response = await client.PostAsJsonAsync("/v2/payment-requests", body, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create PayOS payment link for user {UserId}", userId);
            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("PayOS returned non-success status {StatusCode} for user {UserId}: {Body}", response.StatusCode, userId, errorBody);
            throw new InvalidOperationException("Failed to create PayOS payment link");
        }

        using var contentStream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await System.Text.Json.JsonDocument.ParseAsync(contentStream, cancellationToken: ct);
        var root = doc.RootElement;

        // Kiểm tra mã code trong body PayOS, nhiều trường hợp lỗi nhưng vẫn HTTP 200
        if (root.TryGetProperty("code", out var codeProp))
        {
            var code = codeProp.GetString();
            if (!string.Equals(code, "00", StringComparison.OrdinalIgnoreCase))
            {
                var desc = root.TryGetProperty("desc", out var descProp) ? descProp.GetString() : null;
                _logger.LogError("PayOS returned error code {Code} for user {UserId}: {Desc}", code, userId, desc);
                throw new InvalidOperationException($"Failed to create PayOS payment link: {desc ?? "Unknown error"}");
            }
        }

        // Expect structure similar to:
        // { "data": { "paymentLinkId": "...", "orderCode": "...", "bin": "...", "accountNumber": "...", "accountName": "...", "description": "...", "amount": 100000 }, "signature": "..." }
        var dataElement = root.TryGetProperty("data", out var d) ? d : root;

        // checkoutUrl có thể không tồn tại trong response PayOS -> không được GetProperty trực tiếp
        var checkoutUrl = dataElement.TryGetProperty("checkoutUrl", out var cuProp)
            ? (cuProp.GetString() ?? string.Empty)
            : string.Empty;

        // Ưu tiên paymentLinkId, fallback sang id nếu có
        string? paymentLinkId = null;
        if (dataElement.TryGetProperty("paymentLinkId", out var plProp))
        {
            paymentLinkId = plProp.GetString();
        }

        string? paymentId = paymentLinkId;
        if (string.IsNullOrEmpty(paymentId) && dataElement.TryGetProperty("id", out var idProp))
        {
            paymentId = idProp.GetString();
        }

        // Lấy lại orderCode từ response nếu có (ưu tiên server-side)
        if (dataElement.TryGetProperty("orderCode", out var ocProp))
        {
            // PayOS trả về dạng số, đảm bảo không vượt quá giới hạn long
            if (ocProp.ValueKind is System.Text.Json.JsonValueKind.Number &&
                ocProp.TryGetInt64(out var ocLong))
            {
                orderCode = ocLong;
            }
        }

        // Lấy qrCode thô từ PayOS nếu có
        string? qrCode = null;
        if (dataElement.TryGetProperty("qrCode", out var qrProp))
        {
            qrCode = qrProp.GetString();
        }

        // Dữ liệu để build QR (nếu PayOS trả về)
        string? qrImageUrl = null;
        if (dataElement.TryGetProperty("bin", out var binProp) &&
            dataElement.TryGetProperty("accountNumber", out var accProp) &&
            dataElement.TryGetProperty("accountName", out var nameProp))
        {
            var bin = binProp.GetString();
            var accountNumber = accProp.GetString();
            var accountName = nameProp.GetString();
            var descForQr = dataElement.TryGetProperty("description", out var descProp) ? descProp.GetString() : description;
            // Luôn dùng amount mà backend đang gửi (request.Amount hoặc default),
            // tránh trường hợp PayOS trả amount khác khiến QR lệch số tiền user yêu cầu
            var amountForQr = amount;

            if (!string.IsNullOrWhiteSpace(bin) &&
                !string.IsNullOrWhiteSpace(accountNumber) &&
                !string.IsNullOrWhiteSpace(accountName))
            {
                // Dùng VietQR template giống như bạn dùng trong Postman visualizer
                // https://api.vietqr.io/image/{bin}-{accountNumber}-vietqr_pro.jpg?addInfo={description}&amount={amount}&accountName={accountName}
                var addInfo = Uri.EscapeDataString(descForQr ?? string.Empty);
                var accNameEnc = Uri.EscapeDataString(accountName);
                qrImageUrl = $"https://api.vietqr.io/image/{bin}-{accountNumber}-vietqr_pro.jpg?addInfo={addInfo}&amount={amountForQr}&accountName={accNameEnc}";
            }
        }

        return new CreatePremiumPaymentResponse
        {
            CheckoutUrl = checkoutUrl,
            OrderCode = orderCode.ToString(),
            PaymentId = paymentId,
            PaymentLinkId = paymentLinkId,
            QrCode = qrCode,
            QrImageUrl = qrImageUrl
        };
    }

    public async Task<Guid?> HandlePayOsWebhookAsync(string rawBody, string signature, CancellationToken ct)
    {
        // Validate signature if checksum key is configured.
        if (!string.IsNullOrWhiteSpace(_options.ChecksumKey))
        {
            if (!IsValidSignature(rawBody, signature, _options.ChecksumKey))
            {
                _logger.LogWarning("Invalid PayOS webhook signature");
                return null;
            }
        }

        // The exact webhook model depends on PayOS; here we parse minimal fields.
        PayOsWebhookDto? payload;
        try
        {
            payload = System.Text.Json.JsonSerializer.Deserialize<PayOsWebhookDto>(rawBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize PayOS webhook payload");
            return null;
        }

        if (payload is null || string.IsNullOrWhiteSpace(payload.OrderCode))
        {
            _logger.LogWarning("PayOS webhook payload missing orderCode");
            return null;
        }

        // Only proceed on successful payment
        var status = payload.Status?.ToUpperInvariant();
        if (status is not ("PAID" or "SUCCESS" or "COMPLETED"))
        {
            _logger.LogInformation("Ignoring PayOS webhook with non-success status {Status} for order {OrderCode}", payload.Status, payload.OrderCode);
            return null;
        }

        // Our orderCode format: PREMIUM-{userId}-{timestamp}
        Guid userId;
        try
        {
            var parts = payload.OrderCode.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                _logger.LogWarning("Unexpected orderCode format: {OrderCode}", payload.OrderCode);
                return null;
            }

            userId = Guid.Parse(parts[1]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse user id from PayOS orderCode {OrderCode}", payload.OrderCode);
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            _logger.LogWarning("User not found for PayOS orderCode {OrderCode}", payload.OrderCode);
            return null;
        }

        // Update subscription
        user.SubscriptionType = _options.PremiumSubscriptionType;
        var now = DateTime.UtcNow;
        if (user.SubscriptionExpiresAt.HasValue && user.SubscriptionExpiresAt.Value > now)
        {
            user.SubscriptionExpiresAt = user.SubscriptionExpiresAt.Value.AddDays(_options.PremiumDays);
        }
        else
        {
            user.SubscriptionExpiresAt = now.AddDays(_options.PremiumDays);
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to update subscription for user {UserId} after PayOS payment", userId);
            return null;
        }

        _logger.LogInformation("Upgraded user {UserId} to premium via PayOS order {OrderCode}", userId, payload.OrderCode);
        return userId;
    }

    private static bool IsValidSignature(string rawBody, string signature, string key)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody));
        var computed = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return string.Equals(computed, signature.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}


