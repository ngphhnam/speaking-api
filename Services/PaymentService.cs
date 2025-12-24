using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Payments;
using SpeakingPractice.Api.Options;
using SpeakingPractice.Api.Services.Interfaces;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Services;

public class PaymentService(
    IHttpClientFactory httpClientFactory,
    IOptions<PayOsOptions> payOsOptions,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    ILogger<PaymentService> logger,
    Microsoft.AspNetCore.SignalR.IHubContext<SpeakingPractice.Api.Hubs.PaymentHub> hubContext) : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly PayOsOptions _options = payOsOptions.Value;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<PaymentService> _logger = logger;
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly Microsoft.AspNetCore.SignalR.IHubContext<SpeakingPractice.Api.Hubs.PaymentHub> _hubContext = hubContext;

    public async Task<CreatePremiumPaymentResponse> CreatePremiumPaymentAsync(
        Guid userId,
        CreatePremiumPaymentRequest request,
        CancellationToken ct)
    {
        var planCode = string.IsNullOrWhiteSpace(request.PlanCode) ? "premium_1m" : request.PlanCode!;
        var planDays = ResolvePlanDays(planCode);
        var planPrice = request.Amount ?? ResolvePlanPrice(planCode, planDays);

        // PayOS yêu cầu orderCode là số nguyên dương, <= 9007199254740991
        // -> dùng timestamp milliseconds để vừa duy nhất vừa đúng constraint.
        // (Nếu sau này cần map userId <-> orderCode, ta lưu mapping trong DB thay vì nhét vào chuỗi orderCode)
        long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Default amount if not provided – you can adjust this to your real premium price
        var amount = planPrice; // example pricing based on plan

        // PayOS giới hạn description tối đa 25 ký tự
        var description = GenerateDescription();
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

        var rawResponse = await response.Content.ReadAsStringAsync(ct);
        using var doc = System.Text.Json.JsonDocument.Parse(rawResponse);
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

        var currency = dataElement.TryGetProperty("currency", out var curProp)
            ? (curProp.GetString() ?? "VND")
            : "VND";

        var now = DateTimeOffset.UtcNow;
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderCode = orderCode.ToString(),
            ClientReference = request.ClientReference,
            PaymentLinkId = paymentLinkId,
            PaymentId = paymentId,
            Amount = amount,
            Currency = currency,
            Description = description,
            Status = "pending",
            Provider = "PayOS",
            CheckoutUrl = checkoutUrl,
            QrCode = qrCode,
            QrImageUrl = qrImageUrl,
            PlanCode = planCode,
            PlanDays = planDays,
            PlanPrice = planPrice,
            ExpiredAt = DateTimeOffset.FromUnixTimeSeconds(expiredAt),
            ProviderResponse = rawResponse,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _dbContext.Payments.AddAsync(payment, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new CreatePremiumPaymentResponse
        {
            CheckoutUrl = checkoutUrl,
            OrderCode = orderCode.ToString(),
            PaymentId = paymentId,
            PaymentLinkId = paymentLinkId,
            QrCode = qrCode,
            QrImageUrl = qrImageUrl,
            ExpiredAt = DateTimeOffset.FromUnixTimeSeconds(expiredAt),
            BankAccountName = dataElement.TryGetProperty("accountName", out var accNameProp) ? accNameProp.GetString() : null,
            BankAccountNumber = dataElement.TryGetProperty("accountNumber", out var accNumProp) ? accNumProp.GetString() : null,
            Description = description,
            PlanCode = planCode,
            PlanDays = planDays,
            PlanPrice = planPrice
        };
    }

    public async Task<Guid?> HandlePayOsWebhookAsync(string rawBody, string signature, CancellationToken ct)
    {
        // Parse webhook body once (PayOS: code/desc/success + data{orderCode,paymentLinkId,amount,..., signature})
        string orderCode = string.Empty;
        string? paymentLinkId = null;
        long amount = 0;
        bool isSuccess = false;
        string payloadSignature = signature ?? string.Empty;
        System.Text.Json.JsonElement dataElement = default;

        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(rawBody);
            var root = doc.RootElement;
            dataElement = root.TryGetProperty("data", out var d) ? d : root;

            // PayOS đặt signature ở body (top-level), không ở header
            if (string.IsNullOrWhiteSpace(payloadSignature) &&
                root.TryGetProperty("signature", out var sigProp))
            {
                payloadSignature = sigProp.GetString() ?? string.Empty;
            }

            // Validate signature nếu có checksum key cấu hình
            if (!string.IsNullOrWhiteSpace(_options.ChecksumKey))
            {
                var computed = ComputePayOsSignature(dataElement, _options.ChecksumKey);
                _logger.LogInformation("Handling PayOS webhook. Incoming signature: {Signature}, Computed: {Computed}", payloadSignature, computed);
                _logger.LogDebug("PayOS webhook raw body before signature validation: {Body}", rawBody);

                if (!string.Equals(payloadSignature, computed, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Invalid PayOS webhook signature");
                    return null;
                }
            }

            if (dataElement.TryGetProperty("orderCode", out var ocProp))
            {
                orderCode = ocProp.ToString();
            }
            else if (root.TryGetProperty("orderCode", out var ocRootProp))
            {
                orderCode = ocRootProp.ToString();
            }

            if (dataElement.TryGetProperty("paymentLinkId", out var plProp))
            {
                paymentLinkId = plProp.GetString();
            }

            if (dataElement.TryGetProperty("amount", out var amtProp) && amtProp.TryGetInt64(out var amt))
            {
                amount = amt;
            }

            var rootCodeOk = root.TryGetProperty("code", out var rc) && string.Equals(rc.GetString(), "00", StringComparison.OrdinalIgnoreCase);
            var dataCodeOk = dataElement.TryGetProperty("code", out var dc) && string.Equals(dc.GetString(), "00", StringComparison.OrdinalIgnoreCase);
            var rootSuccess = root.TryGetProperty("success", out var succProp) && succProp.ValueKind == System.Text.Json.JsonValueKind.True;
            var dataStatusOk = dataElement.TryGetProperty("status", out var stProp) && stProp.GetString()?.Equals("PAID", StringComparison.OrdinalIgnoreCase) == true;

            isSuccess = rootCodeOk || dataCodeOk || rootSuccess || dataStatusOk;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse PayOS webhook body");
            return null;
        }

        if (string.IsNullOrWhiteSpace(orderCode))
        {
            _logger.LogWarning("PayOS webhook payload missing orderCode");
            return null;
        }

        // Tìm payment đã lưu theo orderCode hoặc paymentLinkId
        var payment = await _dbContext.Payments
            .FirstOrDefaultAsync(p => p.OrderCode == orderCode || (!string.IsNullOrEmpty(paymentLinkId) && p.PaymentLinkId == paymentLinkId), ct);

        if (payment is null)
        {
            _logger.LogWarning("Payment not found for PayOS webhook. orderCode={OrderCode}, paymentLinkId={PaymentLinkId}", orderCode, paymentLinkId);
            return null;
        }

        if (string.Equals(payment.Status, "paid", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Payment already marked as paid. orderCode={OrderCode}", orderCode);
            return payment.UserId;
        }

        payment.Status = isSuccess ? "paid" : "failed";
        if (isSuccess)
        {
            payment.PaidAt = DateTimeOffset.UtcNow;
        }
        payment.ProviderResponse = rawBody;
        payment.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
        await BroadcastPaymentStatusAsync(payment.UserId, payment, ct);

        if (!isSuccess)
        {
            _logger.LogWarning("PayOS webhook not successful. orderCode={OrderCode}", orderCode);
            return null;
        }

        var user = await _userManager.FindByIdAsync(payment.UserId.ToString());
        if (user is null)
        {
            _logger.LogWarning("User not found for PayOS orderCode {OrderCode}", orderCode);
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
        // Update plan info from payment if available
        user.SubscriptionPlanCode = payment.PlanCode ?? user.SubscriptionPlanCode;
        user.SubscriptionPlanDays = payment.PlanDays ?? user.SubscriptionPlanDays;

        user.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to update subscription for user {UserId} after PayOS payment", payment.UserId);
            return null;
        }

        _logger.LogInformation("Upgraded user {UserId} to premium via PayOS order {OrderCode}", payment.UserId, orderCode);
        return payment.UserId;
    }

    public async Task<PaymentStatusResponse?> GetPaymentStatusAsync(Guid userId, string orderCodeOrPaymentLinkId, CancellationToken ct)
    {
        var payment = await _dbContext.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                (p.OrderCode == orderCodeOrPaymentLinkId || p.PaymentLinkId == orderCodeOrPaymentLinkId),
                ct);

        if (payment is null)
        {
            return null;
        }

        return ToStatusResponse(payment);
    }

    private static string ComputePayOsSignature(System.Text.Json.JsonElement dataElement, string checksumKey)
    {
        // PayOS rule: build key=value&... with keys sorted alphabetically, null => empty string
        var keyValues = dataElement
            .EnumerateObject()
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .Select(p =>
            {
                var value = p.Value.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.Null or System.Text.Json.JsonValueKind.Undefined => string.Empty,
                    System.Text.Json.JsonValueKind.String => p.Value.GetString() ?? string.Empty,
                    _ => p.Value.ToString() ?? string.Empty
                };

                if (value == "null" || value == "undefined")
                {
                    value = string.Empty;
                }

                return $"{p.Name}={value}";
            });

        var message = string.Join("&", keyValues);
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static PaymentStatusResponse ToStatusResponse(Payment payment) => new()
    {
        OrderCode = payment.OrderCode,
        PaymentLinkId = payment.PaymentLinkId,
        Status = payment.Status,
        Amount = payment.Amount,
        Currency = payment.Currency,
        Provider = payment.Provider,
        CheckoutUrl = payment.CheckoutUrl,
        QrCode = payment.QrCode,
        QrImageUrl = payment.QrImageUrl,
        PaidAt = payment.PaidAt,
        CreatedAt = payment.CreatedAt,
        UpdatedAt = payment.UpdatedAt
    };

    private static string GenerateDescription()
    {
        // Random-ish suffix to avoid identical descriptions while staying under 25 chars
        var rnd = Random.Shared.Next(1000, 9999);
        return $"Nâng cấp premium #{rnd}".Substring(0, Math.Min(25, $"Nâng cấp premium #{rnd}".Length));
    }

    private static int ResolvePlanDays(string planCode)
    {
        return planCode switch
        {
            "premium_3m" => 90,
            "premium_6m" => 180,
            "premium_12m" => 365,
            _ => 30 // premium_1m default
        };
    }

    private static long ResolvePlanPrice(string planCode, int planDays)
    {
        // Simple pricing model: 10_000 VND per 30 days, proportional to planDays. Adjust as needed.
        var pricePer30Days = 10_000L;
        return pricePer30Days * planDays / 30;
    }

    private Task BroadcastPaymentStatusAsync(Guid userId, Payment payment, CancellationToken ct)
    {
        var payload = ToStatusResponse(payment);
        // Broadcast to user-specific group and user channel for flexibility
        return Task.WhenAll(
            _hubContext.Clients.User(userId.ToString()).SendAsync("paymentUpdated", payload, ct),
            _hubContext.Clients.Group(userId.ToString()).SendAsync("paymentUpdated", payload, ct)
        );
    }
}


