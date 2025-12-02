using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.SpeakingSessions;
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
            return Unauthorized();
        }

        var result = await speakingSessionService.CreateSessionAsync(
            request,
            userId.Value,
            ct);

        logger.LogInformation("User {UserId} created speaking session {SessionId}", userId, result.Id);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions([FromQuery] Guid? userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");
        var targetUserId = isAdmin && userId.HasValue ? userId.Value : requesterId.Value;

        var sessions = await speakingSessionService.GetUserSessionsAsync(targetUserId, ct);
        return Ok(sessions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionRequest? request, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return NotFound();
        }

        // Update session status or completion
        if (request is not null)
        {
            // Implementation would update session based on request
            logger.LogInformation("Updating session {SessionId}", id);
        }

        return Ok(session);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return NotFound();
        }

        // Implementation would delete session
        logger.LogInformation("Deleting session {SessionId}", id);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        var session = await speakingSessionService.GetByIdAsync(id, requesterId.Value, User.IsInRole("Admin"), ct);
        if (session is null)
        {
            return NotFound();
        }

        // Implementation would mark session as completed
        logger.LogInformation("Completing session {SessionId}", id);
        return Ok(session);
    }

    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
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

        return Ok(statistics);
    }

    [HttpGet("user/{userId:guid}/active")]
    public async Task<IActionResult> GetActive(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var activeSessions = sessions.Where(s => s.Status == "in_progress");
        return Ok(activeSessions);
    }

    [HttpGet("user/{userId:guid}/completed")]
    public async Task<IActionResult> GetCompleted(Guid userId, CancellationToken ct)
    {
        var requesterId = GetUserId();
        if (requesterId is null)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var sessions = await speakingSessionService.GetUserSessionsAsync(userId, ct);
        var completedSessions = sessions.Where(s => s.Status == "completed");
        return Ok(completedSessions);
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

