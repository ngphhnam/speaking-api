using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/streak")]
public class StreakController(
    IStreakService streakService,
    IAchievementService achievementService,
    ILogger<StreakController> logger) : ControllerBase
{
    [HttpGet("info")]
    public async Task<IActionResult> GetStreakInfo(CancellationToken ct)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var streakInfo = await streakService.GetStreakInfoAsync(userId.Value, ct);
        if (streakInfo == null)
        {
            return this.ApiNotFound(ErrorCodes.USER_NOT_FOUND_RESOURCE, "User not found");
        }

        return this.ApiOk(streakInfo, "Streak information retrieved successfully");
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetStreakHistory(CancellationToken ct)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var history = await streakService.GetStreakHistoryAsync(userId.Value, ct);
        return this.ApiOk(history, "Streak history retrieved successfully");
    }

    [HttpGet("level")]
    public async Task<IActionResult> GetUserLevel(CancellationToken ct)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var levelInfo = await achievementService.GetUserLevelInfoAsync(userId.Value, ct);
        if (levelInfo == null)
        {
            return this.ApiNotFound(ErrorCodes.USER_NOT_FOUND_RESOURCE, "User not found");
        }

        return this.ApiOk(levelInfo, "User level information retrieved successfully");
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}



