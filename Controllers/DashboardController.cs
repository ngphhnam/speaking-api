using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.Dashboard;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/statistic")]
public class DashboardController(
    ApplicationDbContext context,
    IAnalysisResultRepository analysisResultRepository,
    ILogger<DashboardController> logger) : ControllerBase
{
    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetTotalStatistics(Guid userId, CancellationToken ct = default)
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

        try
        {
            // Get user info (streak, last practice date)
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.CurrentStreak,
                    u.LastPracticeDate
                })
                .FirstOrDefaultAsync(ct);

            if (user == null)
            {
                return this.ApiNotFound(ErrorCodes.NOT_FOUND, "User not found");
            }

            // Get total questions attempted (unique questions)
            var totalQuestions = await context.Recordings
                .Where(r => r.UserId == userId && r.QuestionId.HasValue)
                .Select(r => r.QuestionId!.Value)
                .Distinct()
                .CountAsync(ct);

            // Get total recordings
            var totalRecordings = await context.Recordings
                .Where(r => r.UserId == userId)
                .CountAsync(ct);

            // Get average score from analysis results
            var averageScore = await context.AnalysisResults
                .Where(a => a.UserId == userId && a.OverallBandScore.HasValue)
                .Select(a => a.OverallBandScore!.Value)
                .DefaultIfEmpty()
                .AverageAsync(ct);

            // Get total practice time (sum of session durations in minutes)
            var totalPracticeTime = await context.PracticeSessions
                .Where(s => s.UserId == userId && s.TotalDurationSeconds.HasValue)
                .SumAsync(s => s.TotalDurationSeconds!.Value / 60, ct);

            // Get last practice date - ưu tiên từ user.LastPracticeDate (được cập nhật bởi StreakService)
            // Nếu không có, fallback sang most recent recording hoặc practice session
            string? lastPracticeDate = null;
            
            if (user.LastPracticeDate.HasValue)
            {
                // Convert DateOnly to DateTimeOffset (end of day UTC)
                var lastPracticeDateTime = new DateTimeOffset(
                    user.LastPracticeDate.Value.Year,
                    user.LastPracticeDate.Value.Month,
                    user.LastPracticeDate.Value.Day,
                    23, 59, 59, TimeSpan.Zero);
                lastPracticeDate = lastPracticeDateTime.ToUniversalTime().ToString("O");
            }
            else
            {
                // Fallback: lấy từ most recent recording
                var lastRecording = await context.Recordings
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.RecordedAt)
                    .Select(r => r.RecordedAt)
                    .FirstOrDefaultAsync(ct);

                if (lastRecording != default)
                {
                    lastPracticeDate = lastRecording.ToUniversalTime().ToString("O");
                }
                else
                {
                    // Fallback: lấy từ most recent practice session
                    var lastSession = await context.PracticeSessions
                        .Where(s => s.UserId == userId)
                        .OrderByDescending(s => s.CompletedAt ?? s.StartedAt)
                        .Select(s => s.CompletedAt ?? s.StartedAt)
                        .FirstOrDefaultAsync(ct);

                    if (lastSession != default)
                    {
                        lastPracticeDate = lastSession.ToUniversalTime().ToString("O");
                    }
                }
            }

            var statistics = new TotalStatisticsDto
            {
                TotalQuestions = totalQuestions,
                TotalRecordings = totalRecordings,
                AverageScore = averageScore > 0 ? (decimal?)Math.Round(averageScore, 1) : null,
                TotalPracticeTime = (int)totalPracticeTime,
                StreakDays = user.CurrentStreak,
                LastPracticeDate = lastPracticeDate
            };

            return this.ApiOk(statistics, "Total statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving total statistics for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "An error occurred while retrieving statistics");
        }
    }

    [HttpGet("user/{userId:guid}/scores-by-skill")]
    public async Task<IActionResult> GetScoresBySkill(Guid userId, CancellationToken ct = default)
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

        try
        {
            var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);
            var analysesList = analyses.ToList();

            if (!analysesList.Any())
            {
                return this.ApiOk(new ScoresBySkillDto(), "Scores by skill retrieved successfully");
            }

            var fluencyScores = analysesList
                .Where(a => a.FluencyScore.HasValue)
                .Select(a => a.FluencyScore!.Value)
                .ToList();

            var vocabularyScores = analysesList
                .Where(a => a.VocabularyScore.HasValue)
                .Select(a => a.VocabularyScore!.Value)
                .ToList();

            var grammarScores = analysesList
                .Where(a => a.GrammarScore.HasValue)
                .Select(a => a.GrammarScore!.Value)
                .ToList();

            var pronunciationScores = analysesList
                .Where(a => a.PronunciationScore.HasValue)
                .Select(a => a.PronunciationScore!.Value)
                .ToList();

            var scores = new ScoresBySkillDto
            {
                Fluency = fluencyScores.Any() ? (decimal?)Math.Round(fluencyScores.Average(), 1) : null,
                Vocabulary = vocabularyScores.Any() ? (decimal?)Math.Round(vocabularyScores.Average(), 1) : null,
                Grammar = grammarScores.Any() ? (decimal?)Math.Round(grammarScores.Average(), 1) : null,
                Pronunciation = pronunciationScores.Any() ? (decimal?)Math.Round(pronunciationScores.Average(), 1) : null
            };

            return this.ApiOk(scores, "Scores by skill retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving scores by skill for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "An error occurred while retrieving scores by skill");
        }
    }

    [HttpGet("user/{userId:guid}/progress-by-month")]
    public async Task<IActionResult> GetProgressByMonth(Guid userId, CancellationToken ct = default)
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

        try
        {
            var now = DateTimeOffset.UtcNow;
            var threeMonthsAgo = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(-3);
            
            // Get recordings from last 4 months (including current month)
            var recordings = await context.Recordings
                .Where(r => r.UserId == userId && r.RecordedAt >= threeMonthsAgo)
                .Include(r => r.AnalysisResult)
                .ToListAsync(ct);

            // Group by month and calculate statistics
            var monthlyProgress = recordings
                .GroupBy(r => new { r.RecordedAt.Year, r.RecordedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Recordings = g.ToList(),
                    AverageScore = g
                        .Where(r => r.AnalysisResult != null && r.AnalysisResult.OverallBandScore.HasValue)
                        .Select(r => r.AnalysisResult!.OverallBandScore!.Value)
                        .DefaultIfEmpty()
                        .Average()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var viCulture = new CultureInfo("vi-VN");
            var result = monthlyProgress.Select(mp => new MonthlyProgressDto
            {
                Month = new DateTime(mp.Year, mp.Month, 1).ToString("MMM yyyy", viCulture),
                AverageScore = mp.AverageScore > 0 ? (decimal?)Math.Round(mp.AverageScore, 1) : null,
                TotalRecordings = mp.Recordings.Count
            }).ToList();

            // Ensure we have last 4 months (fill missing months with 0)
            var last4Months = new List<MonthlyProgressDto>();
            for (int i = 3; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var monthKey = new DateTime(monthDate.Year, monthDate.Month, 1).ToString("MMM yyyy", viCulture);
                var existing = result.FirstOrDefault(r => r.Month == monthKey);
                
                if (existing != null)
                {
                    last4Months.Add(existing);
                }
                else
                {
                    last4Months.Add(new MonthlyProgressDto
                    {
                        Month = monthKey,
                        AverageScore = null,
                        TotalRecordings = 0
                    });
                }
            }

            return this.ApiOk(last4Months, "Progress by month retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving progress by month for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "An error occurred while retrieving progress by month");
        }
    }

    [HttpGet("user/{userId:guid}/weak-topics")]
    public async Task<IActionResult> GetWeakTopics(Guid userId, CancellationToken ct = default)
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

        try
        {
            // Get sessions with topics and their scores
            var sessions = await context.PracticeSessions
                .Where(s => s.UserId == userId && s.TopicId.HasValue && s.OverallBandScore.HasValue)
                .Include(s => s.Topic)
                .ToListAsync(ct);

            if (!sessions.Any())
            {
                return this.ApiOk(new List<WeakTopicDto>(), "Weak topics retrieved successfully");
            }

            // Group by topic and calculate average score
            var topicStats = sessions
                .GroupBy(s => new { TopicId = s.TopicId!.Value, s.Topic!.Title })
                .Select(g => new WeakTopicDto
                {
                    TopicId = g.Key.TopicId,
                    TopicTitle = g.Key.Title,
                    AverageScore = (decimal?)Math.Round(g.Average(s => s.OverallBandScore!.Value), 1),
                    TotalAttempts = g.Count()
                })
                .OrderBy(t => t.AverageScore)
                .Take(3) // Top 3 weakest topics
                .ToList();

            return this.ApiOk(topicStats, "Weak topics retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving weak topics for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "An error occurred while retrieving weak topics");
        }
    }

    [HttpGet("user/{userId:guid}/recent-recordings")]
    public async Task<IActionResult> GetRecentRecordings(Guid userId, CancellationToken ct = default)
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

        try
        {
            var recordings = await context.Recordings
                .Where(r => r.UserId == userId && r.QuestionId.HasValue)
                .Include(r => r.Question)
                .Include(r => r.AnalysisResult)
                .OrderByDescending(r => r.RecordedAt)
                .Take(5)
                .ToListAsync(ct);

            var result = recordings.Select(r => new RecentRecordingDto
            {
                Id = r.Id,
                QuestionId = r.QuestionId!.Value,
                QuestionText = r.Question?.QuestionText ?? string.Empty,
                RecordedAt = r.RecordedAt.ToUniversalTime().ToString("O"),
                OverallScore = r.AnalysisResult?.OverallBandScore
            }).ToList();

            return this.ApiOk(result, "Recent recordings retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving recent recordings for user {UserId}", userId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "An error occurred while retrieving recent recordings");
        }
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}

