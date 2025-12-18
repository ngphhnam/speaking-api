using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/user-settings")]
public class UserSettingsController(
    ApplicationDbContext context,
    ILogger<UserSettingsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSettings(CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await context.Users.FindAsync(new object[] { userId.Value }, ct);
        if (user is null)
        {
            return this.ApiNotFound(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        var settings = new UserSettingsDto
        {
            UserId = user.Id,
            TargetBandScore = user.TargetBandScore,
            CurrentLevel = user.CurrentLevel,
            ExamDate = user.ExamDate,
            EmailVerified = user.EmailVerified,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            SubscriptionType = user.SubscriptionType,
            SubscriptionExpiresAt = user.SubscriptionExpiresAt
        };

        return this.ApiOk(settings, "Settings retrieved successfully");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateUserSettingsRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var user = await context.Users.FindAsync(new object[] { userId.Value }, ct);
        if (user is null)
        {
            return this.ApiNotFound(ErrorCodes.USER_NOT_FOUND, "User not found");
        }

        if (request.TargetBandScore.HasValue) user.TargetBandScore = request.TargetBandScore;
        if (request.CurrentLevel is not null) user.CurrentLevel = request.CurrentLevel;
        if (request.ExamDate.HasValue) user.ExamDate = request.ExamDate;

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Updated settings for user {UserId}", userId);
        return this.ApiOk("Settings updated successfully");
    }

    [HttpGet("notification-preferences")]
    public async Task<IActionResult> GetNotificationPreferences(CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        // For now, return default preferences
        // In production, you'd have a UserNotificationPreferences table
        var preferences = new NotificationPreferencesDto
        {
            UserId = userId.Value,
            EmailNotifications = true,
            PracticeReminders = true,
            AchievementNotifications = true,
            WeeklySummary = true,
            StreakReminders = true
        };

        return this.ApiOk(preferences, "Notification preferences retrieved successfully");
    }

    [HttpPut("notification-preferences")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferencesRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        // In production, save to database
        logger.LogInformation("Updated notification preferences for user {UserId}", userId);
        return this.ApiOk("Notification preferences updated successfully");
    }

    [HttpGet("practice-preferences")]
    public async Task<IActionResult> GetPracticePreferences(CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        // For now, return default preferences
        var preferences = new PracticePreferencesDto
        {
            UserId = userId.Value,
            RecordingQuality = "high",
            AutoSubmit = false,
            FeedbackDetailLevel = "detailed",
            PreferredAIModel = "llama",
            ShowHints = true,
            EnableTimer = true
        };

        return this.ApiOk(preferences, "Practice preferences retrieved successfully");
    }

    [HttpPut("practice-preferences")]
    public async Task<IActionResult> UpdatePracticePreferences([FromBody] UpdatePracticePreferencesRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        // In production, save to database
        logger.LogInformation("Updated practice preferences for user {UserId}", userId);
        return this.ApiOk("Practice preferences updated successfully");
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}

public class UserSettingsDto
{
    public Guid UserId { get; set; }
    public decimal? TargetBandScore { get; set; }
    public string? CurrentLevel { get; set; }
    public DateOnly? ExamDate { get; set; }
    public bool EmailVerified { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string SubscriptionType { get; set; } = "free";
    public DateTime? SubscriptionExpiresAt { get; set; }
}

public class UpdateUserSettingsRequest
{
    public decimal? TargetBandScore { get; set; }
    public string? CurrentLevel { get; set; }
    public DateOnly? ExamDate { get; set; }
}

public class NotificationPreferencesDto
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PracticeReminders { get; set; }
    public bool AchievementNotifications { get; set; }
    public bool WeeklySummary { get; set; }
    public bool StreakReminders { get; set; }
}

public class UpdateNotificationPreferencesRequest
{
    public bool? EmailNotifications { get; set; }
    public bool? PracticeReminders { get; set; }
    public bool? AchievementNotifications { get; set; }
    public bool? WeeklySummary { get; set; }
    public bool? StreakReminders { get; set; }
}

public class PracticePreferencesDto
{
    public Guid UserId { get; set; }
    public string RecordingQuality { get; set; } = "high";
    public bool AutoSubmit { get; set; }
    public string FeedbackDetailLevel { get; set; } = "detailed";
    public string PreferredAIModel { get; set; } = "llama";
    public bool ShowHints { get; set; }
    public bool EnableTimer { get; set; }
}

public class UpdatePracticePreferencesRequest
{
    public string? RecordingQuality { get; set; }
    public bool? AutoSubmit { get; set; }
    public string? FeedbackDetailLevel { get; set; }
    public string? PreferredAIModel { get; set; }
    public bool? ShowHints { get; set; }
    public bool? EnableTimer { get; set; }
}


