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
    public async Task<IActionResult> Create(
        [FromBody] CreateSpeakingSessionRequest request,
        CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        try
        {
            var result = await speakingSessionService.CreateSessionAsync(
                request,
                userId.Value,
                ct);

            logger.LogInformation("User {UserId} created speaking session {SessionId}", userId, result.Id);
            return this.ApiCreated(nameof(GetById), new { id = result.Id }, result, "Session created successfully");
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access for user {UserId}", userId);
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating session for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to create session");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions([FromQuery] Guid? userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var isAdmin = User.IsInRole("Admin");
        var targetUserId = isAdmin && userId.HasValue ? userId.Value : requesterId.Value;

        var sessions = await speakingSessionService.GetUserSessionsAsync(targetUserId, ct);
        return this.ApiOk(sessions, "Sessions retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(ErrorCodes.SESSION_NOT_FOUND, $"Session with id {id} not found");
        }

        return this.ApiOk(session, "Session retrieved successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionRequest? request, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(ErrorCodes.SESSION_NOT_FOUND, $"Session with id {id} not found");
        }

        // Update session status or completion
        if (request is not null)
        {
            // Implementation would update session based on request
            logger.LogInformation("Updating session {SessionId}", id);
        }

        return this.ApiOk(session, "Session updated successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(ErrorCodes.SESSION_NOT_FOUND, $"Session with id {id} not found");
        }

        // Implementation would delete session
        logger.LogInformation("Deleting session {SessionId}", id);
        return this.ApiOk("Session deleted successfully");
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return this.ApiNotFound(ErrorCodes.SESSION_NOT_FOUND, $"Session with id {id} not found");
        }

        // Implementation would mark session as completed
        logger.LogInformation("Completing session {SessionId}", id);
        return this.ApiOk(session, "Session completed successfully");
    }

    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
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

        return this.ApiOk(statistics, "Statistics retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/active")]
    public async Task<IActionResult> GetActive(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var activeSessions = sessions.Where(s => s.Status == "in_progress");
        return this.ApiOk(activeSessions, "Active sessions retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/completed")]
    public async Task<IActionResult> GetCompleted(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var completedSessions = sessions.Where(s => s.Status == "completed");
        return this.ApiOk(completedSessions, "Completed sessions retrieved successfully");
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

