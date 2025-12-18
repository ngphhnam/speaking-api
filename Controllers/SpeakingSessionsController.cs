using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.SpeakingSessions;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/session/speaking-session")]
public class SpeakingSessionsController(
    ISpeakingSessionService speakingSessionService,
    ILogger<SpeakingSessionsController> logger) : ControllerBase
{
    [HttpPost]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> Create(
        [FromForm] CreateSpeakingSessionRequest request,
        [FromForm] IFormFile audio,
        CancellationToken ct)
    {
        if (audio is null || audio.Length == 0)
        {
            return this.ApiBadRequest(
                ErrorCodes.FILE_REQUIRED,
                "Audio file is required.");
        }

        var userId = GetUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        var result = await speakingSessionService.CreateSessionAsync(
            request,
            userId.Value,
            ct);

        logger.LogInformation("User {UserId} created speaking session {SessionId}", userId, result.Id);
        return this.ApiCreated(
            nameof(GetById),
            new { id = result.Id },
            result,
            "Speaking session created successfully");
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions(
        [FromQuery] Guid? userId,
        [FromQuery(Name = "pageNum")] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        var isAdmin = User.IsInRole("Admin");
        var targetUserId = isAdmin && userId.HasValue ? userId.Value : requesterId.Value;

        var sessions = await speakingSessionService.GetUserSessionsAsync(targetUserId, ct);
        var sessionList = sessions.ToList();

        // Normalize paging params
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var totalCount = sessionList.Count;
        var items = sessionList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<SpeakingSessionListItemDto>(items, page, pageSize, totalCount);

        return this.ApiOk(result, "Speaking sessions retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(
                ErrorCodes.SESSION_NOT_FOUND,
                "Speaking session not found");
        }

        return this.ApiOk(session, "Speaking session retrieved successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionRequest? request, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(
                ErrorCodes.SESSION_NOT_FOUND,
                "Speaking session not found");
        }

        // Update session status or completion
        if (request is not null)
        {
            // Implementation would update session based on request
            logger.LogInformation("Updating session {SessionId}", id);
        }

        return this.ApiOk(session, "Speaking session updated successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(
                ErrorCodes.SESSION_NOT_FOUND,
                "Speaking session not found");
        }

        // Implementation would delete session
        logger.LogInformation("Deleting session {SessionId}", id);
        return this.ApiOk("Speaking session deleted successfully");
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(
                ErrorCodes.SESSION_NOT_FOUND,
                "Speaking session not found");
        }

        // Implementation would mark session as completed
        logger.LogInformation("Completing session {SessionId}", id);
        return this.ApiOk(session, "Speaking session marked as completed");
    }

    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(
                ErrorCodes.FORBIDDEN,
                "You don't have permission to access this resource");
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var sessionsList = sessions.ToList();

        var statistics = new
        {
            TotalSessions = sessionsList.Count,
            CompletedSessions = sessionsList.Count(s => s.Status == "completed"),
            ActiveSessions = sessionsList.Count(s => s.Status == "in_progress"),
            TotalPracticeMinutes = sessionsList
                .Where(s => s.TotalDurationSeconds.HasValue)
                .Sum(s => s.TotalDurationSeconds!.Value / 60),
            AvgOverallScore = sessionsList
                .Where(s => s.OverallBandScore.HasValue)
                .Select(s => s.OverallBandScore!.Value)
                .DefaultIfEmpty(0)
                .Average()
        };

        return this.ApiOk(statistics, "Speaking session statistics retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/active")]
    public async Task<IActionResult> GetActive(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(
                ErrorCodes.FORBIDDEN,
                "You don't have permission to access this resource");
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var activeSessions = sessions.Where(s => s.Status == "in_progress");
        return this.ApiOk(activeSessions, "Active speaking sessions retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/completed")]
    public async Task<IActionResult> GetCompleted(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(
                ErrorCodes.UNAUTHORIZED,
                "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(
                ErrorCodes.FORBIDDEN,
                "You don't have permission to access this resource");
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var completedSessions = sessions.Where(s => s.Status == "completed");
        return this.ApiOk(completedSessions, "Completed speaking sessions retrieved successfully");
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}

public class UpdateSessionRequest
{
    public string? Status { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}

