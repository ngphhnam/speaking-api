using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.DTOs.Achievements;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.UserAchievements;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/user-achievements")]
public class UserAchievementsController(
    IUserAchievementRepository userAchievementRepository,
    IAchievementRepository achievementRepository,
    ApplicationDbContext context,
    ILogger<UserAchievementsController> logger) : ControllerBase
{
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserAchievements(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userAchievements = await context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId)
            .ToListAsync(ct);

        return this.ApiOk(userAchievements.Select(MapToDto), "User achievements retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/completed")]
    public async Task<IActionResult> GetCompleted(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userAchievements = await context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId && ua.IsCompleted)
            .ToListAsync(ct);

        return this.ApiOk(userAchievements.Select(MapToDto), "User achievements retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/in-progress")]
    public async Task<IActionResult> GetInProgress(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userAchievements = await context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId && !ua.IsCompleted)
            .ToListAsync(ct);

        return this.ApiOk(userAchievements.Select(MapToDto), "User achievements retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/progress/{achievementId:guid}")]
    public async Task<IActionResult> GetProgress(Guid userId, Guid achievementId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userAchievement = await context.UserAchievements
            .Include(ua => ua.Achievement)
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId, ct);

        if (userAchievement is null)
        {
            // Return progress for achievement not yet started
            var achievement = await achievementRepository.GetByIdAsync(achievementId, ct);
            if (achievement is null)
            {
                return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Achievement not found");
            }

            return this.ApiOk(new
            {
                AchievementId = achievementId,
                IsCompleted = false,
                Progress = "{}",
                EarnedAt = (DateTimeOffset?)null
            }, "Progress retrieved successfully");
        }

        return this.ApiOk(new
        {
            AchievementId = userAchievement.AchievementId,
            IsCompleted = userAchievement.IsCompleted,
            Progress = userAchievement.Progress,
            EarnedAt = userAchievement.EarnedAt
        }, "Progress retrieved successfully");
    }

    [HttpPost("user/{userId:guid}/check")]
    [Authorize(Roles = "Admin")]
    public Task<IActionResult> CheckAchievements(Guid userId, CancellationToken ct = default)
    {
        // This endpoint would check and update achievement progress based on user activity
        // Implementation would analyze user's sessions, recordings, scores, etc.
        logger.LogInformation("Checking achievements for user {UserId}", userId);
        
        return Task.FromResult<IActionResult>(this.ApiOk(new { message = "Achievement check initiated", userId }, "Achievement check initiated"));
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static UserAchievementDto MapToDto(Domain.Entities.UserAchievement userAchievement)
    {
        return new UserAchievementDto
        {
            Id = userAchievement.Id,
            UserId = userAchievement.UserId,
            AchievementId = userAchievement.AchievementId,
            Achievement = new AchievementDto
            {
                Id = userAchievement.Achievement.Id,
                Title = userAchievement.Achievement.Title,
                Description = userAchievement.Achievement.Description,
                AchievementType = userAchievement.Achievement.AchievementType,
                Points = userAchievement.Achievement.Points,
                BadgeIconUrl = userAchievement.Achievement.BadgeIconUrl,
                IsActive = userAchievement.Achievement.IsActive,
                CreatedAt = userAchievement.Achievement.CreatedAt
            },
            Progress = userAchievement.Progress,
            IsCompleted = userAchievement.IsCompleted,
            EarnedAt = userAchievement.EarnedAt,
            CreatedAt = userAchievement.CreatedAt
        };
    }
}
