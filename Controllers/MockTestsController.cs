using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Domain.Enums;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/mock-tests")]
public class MockTestsController(
    ApplicationDbContext context,
    ILogger<MockTestsController> logger) : ControllerBase
{
    [HttpPost("start")]
    public async Task<IActionResult> StartMockTest([FromBody] StartMockTestRequest? request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        request ??= new StartMockTestRequest();

        // Get random questions for Part 1, 2, and 3
        var part1Questions = await context.Questions
            .Where(q => q.IsActive && q.QuestionType == QuestionType.PART1)
            .OrderBy(q => Guid.NewGuid())
            .Take(request.Part1QuestionCount ?? 3)
            .ToListAsync(ct);

        var part2Questions = await context.Questions
            .Where(q => q.IsActive && q.QuestionType == QuestionType.PART2)
            .OrderBy(q => Guid.NewGuid())
            .Take(request.Part2QuestionCount ?? 1)
            .ToListAsync(ct);

        var part3Questions = await context.Questions
            .Where(q => q.IsActive && q.QuestionType == QuestionType.PART3)
            .OrderBy(q => Guid.NewGuid())
            .Take(request.Part3QuestionCount ?? 4)
            .ToListAsync(ct);

        // Create mock test session
        var mockTest = new MockTest
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            Status = "in_progress",
            StartedAt = DateTimeOffset.UtcNow,
            Part1QuestionIds = string.Join(",", part1Questions.Select(q => q.Id)),
            Part2QuestionIds = string.Join(",", part2Questions.Select(q => q.Id)),
            Part3QuestionIds = string.Join(",", part3Questions.Select(q => q.Id)),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.MockTests.Add(mockTest);
        await context.SaveChangesAsync(ct);

        var response = new MockTestDto
        {
            Id = mockTest.Id,
            UserId = mockTest.UserId,
            Status = mockTest.Status,
            StartedAt = mockTest.StartedAt,
            Part1Questions = part1Questions.Select(q => MapQuestionToDto(q)).ToList(),
            Part2Questions = part2Questions.Select(q => MapQuestionToDto(q)).ToList(),
            Part3Questions = part3Questions.Select(q => MapQuestionToDto(q)).ToList()
        };

        logger.LogInformation("User {UserId} started mock test {MockTestId}", userId, mockTest.Id);
        return this.ApiCreated(nameof(GetById), new { id = mockTest.Id }, response, "Mock test started successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var mockTest = await context.MockTests.FindAsync(new object[] { id }, ct);
        if (mockTest is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Mock test not found");
        }

        if (mockTest.UserId != userId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var response = await BuildMockTestDto(mockTest, ct);
        return this.ApiOk(response, "Mock test retrieved successfully");
    }

    [HttpPost("{id:guid}/submit-part")]
    public async Task<IActionResult> SubmitPart(Guid id, [FromBody] SubmitPartRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var mockTest = await context.MockTests.FindAsync(new object[] { id }, ct);
        if (mockTest is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Mock test not found");
        }

        if (mockTest.UserId != userId.Value)
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        // Update completion status
        switch (request.Part)
        {
            case 1:
                mockTest.Part1CompletedAt = DateTimeOffset.UtcNow;
                break;
            case 2:
                mockTest.Part2CompletedAt = DateTimeOffset.UtcNow;
                break;
            case 3:
                mockTest.Part3CompletedAt = DateTimeOffset.UtcNow;
                break;
        }

        mockTest.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} completed part {Part} of mock test {MockTestId}", userId, request.Part, id);
        return this.ApiOk("Part submitted successfully");
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var mockTest = await context.MockTests.FindAsync(new object[] { id }, ct);
        if (mockTest is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Mock test not found");
        }

        if (mockTest.UserId != userId.Value)
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        mockTest.Status = "completed";
        mockTest.CompletedAt = DateTimeOffset.UtcNow;

        // Calculate overall score from recordings
        var allQuestionIds = new List<Guid>();
        allQuestionIds.AddRange(mockTest.Part1QuestionIds.Split(',').Select(Guid.Parse));
        allQuestionIds.AddRange(mockTest.Part2QuestionIds.Split(',').Select(Guid.Parse));
        allQuestionIds.AddRange(mockTest.Part3QuestionIds.Split(',').Select(Guid.Parse));

        var recordings = await context.Recordings
            .Where(r => r.UserId == userId.Value && r.QuestionId.HasValue && allQuestionIds.Contains(r.QuestionId.Value))
            .ToListAsync(ct);

        if (recordings.Any())
        {
            var recordingIds = recordings.Select(r => r.Id).ToList();
            var analyses = await context.AnalysisResults
                .Where(a => recordingIds.Contains(a.RecordingId))
                .ToListAsync(ct);

            if (analyses.Any() && analyses.Any(a => a.OverallBandScore.HasValue))
            {
                mockTest.OverallScore = analyses
                    .Where(a => a.OverallBandScore.HasValue)
                    .Average(a => a.OverallBandScore!.Value);
            }
        }

        mockTest.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} completed mock test {MockTestId} with score {Score}", userId, id, mockTest.OverallScore);
        
        var response = await BuildMockTestDto(mockTest, ct);
        return this.ApiOk(response, "Mock test completed successfully");
    }

    [HttpGet("user/{userId:guid}/history")]
    public async Task<IActionResult> GetUserHistory(Guid userId, CancellationToken ct = default)
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

        var mockTests = await context.MockTests
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.StartedAt)
            .ToListAsync(ct);

        var history = mockTests.Select(m => new MockTestHistoryDto
        {
            Id = m.Id,
            StartedAt = m.StartedAt,
            CompletedAt = m.CompletedAt,
            Status = m.Status,
            OverallScore = m.OverallScore,
            TotalQuestions = m.Part1QuestionIds.Split(',').Length + 
                           m.Part2QuestionIds.Split(',').Length + 
                           m.Part3QuestionIds.Split(',').Length
        }).ToList();

        return this.ApiOk(history, "Mock test history retrieved successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var mockTest = await context.MockTests.FindAsync(new object[] { id }, ct);
        if (mockTest is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Mock test not found");
        }

        if (mockTest.UserId != userId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        context.MockTests.Remove(mockTest);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Deleted mock test {MockTestId}", id);
        return this.ApiOk("Mock test deleted successfully");
    }

    private async Task<MockTestDto> BuildMockTestDto(MockTest mockTest, CancellationToken ct)
    {
        var part1Ids = mockTest.Part1QuestionIds.Split(',').Select(Guid.Parse).ToList();
        var part2Ids = mockTest.Part2QuestionIds.Split(',').Select(Guid.Parse).ToList();
        var part3Ids = mockTest.Part3QuestionIds.Split(',').Select(Guid.Parse).ToList();

        var part1Questions = await context.Questions.Where(q => part1Ids.Contains(q.Id)).ToListAsync(ct);
        var part2Questions = await context.Questions.Where(q => part2Ids.Contains(q.Id)).ToListAsync(ct);
        var part3Questions = await context.Questions.Where(q => part3Ids.Contains(q.Id)).ToListAsync(ct);

        return new MockTestDto
        {
            Id = mockTest.Id,
            UserId = mockTest.UserId,
            Status = mockTest.Status,
            StartedAt = mockTest.StartedAt,
            CompletedAt = mockTest.CompletedAt,
            OverallScore = mockTest.OverallScore,
            Part1Questions = part1Questions.Select(q => MapQuestionToDto(q)).ToList(),
            Part2Questions = part2Questions.Select(q => MapQuestionToDto(q)).ToList(),
            Part3Questions = part3Questions.Select(q => MapQuestionToDto(q)).ToList(),
            Part1CompletedAt = mockTest.Part1CompletedAt,
            Part2CompletedAt = mockTest.Part2CompletedAt,
            Part3CompletedAt = mockTest.Part3CompletedAt
        };
    }

    private static MockTestQuestionDto MapQuestionToDto(Question question)
    {
        return new MockTestQuestionDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType.ToString(), // Convert enum to string
            TimeLimitSeconds = question.TimeLimitSeconds,
            KeyVocabulary = question.KeyVocabulary
        };
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}

// Entity
public class MockTest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = "in_progress"; // in_progress, completed, abandoned
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string Part1QuestionIds { get; set; } = string.Empty;
    public string Part2QuestionIds { get; set; } = string.Empty;
    public string Part3QuestionIds { get; set; } = string.Empty;
    public DateTimeOffset? Part1CompletedAt { get; set; }
    public DateTimeOffset? Part2CompletedAt { get; set; }
    public DateTimeOffset? Part3CompletedAt { get; set; }
    public decimal? OverallScore { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

// DTOs
public class StartMockTestRequest
{
    public int? Part1QuestionCount { get; set; } = 3;
    public int? Part2QuestionCount { get; set; } = 1;
    public int? Part3QuestionCount { get; set; } = 4;
}

public class SubmitPartRequest
{
    public int Part { get; set; } // 1, 2, or 3
}

public class MockTestDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public decimal? OverallScore { get; set; }
    public List<MockTestQuestionDto> Part1Questions { get; set; } = new();
    public List<MockTestQuestionDto> Part2Questions { get; set; } = new();
    public List<MockTestQuestionDto> Part3Questions { get; set; } = new();
    public DateTimeOffset? Part1CompletedAt { get; set; }
    public DateTimeOffset? Part2CompletedAt { get; set; }
    public DateTimeOffset? Part3CompletedAt { get; set; }
}

public class MockTestQuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionType { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public string[]? KeyVocabulary { get; set; }
}

public class MockTestHistoryDto
{
    public Guid Id { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? OverallScore { get; set; }
    public int TotalQuestions { get; set; }
}

