using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.UserProgress;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserProgressController(
    IUserProgressRepository userProgressRepository,
    IAnalysisResultRepository analysisResultRepository,
    IRecordingRepository recordingRepository,
    ISpeakingSessionRepository speakingSessionRepository,
    ILogger<UserProgressController> logger) : ControllerBase
{
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserProgress(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // Get current period progress (today for daily, this week for weekly, this month for monthly)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentDaily = await userProgressRepository.GetByUserAndPeriodAsync(userId, "daily", today, ct);
        
        if (currentDaily is null)
        {
            return Ok(new UserProgressDto
            {
                UserId = userId,
                PeriodType = "daily",
                PeriodStart = today,
                PeriodEnd = today
            });
        }

        return Ok(MapToDto(currentDaily));
    }

    [HttpGet("user/{userId:guid}/daily")]
    public async Task<IActionResult> GetDailyProgress(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var dailyProgress = await userProgressRepository.GetByUserIdAndPeriodTypeAsync(userId, "daily", ct);
        return Ok(dailyProgress.Select(MapToDto));
    }

    [HttpGet("user/{userId:guid}/weekly")]
    public async Task<IActionResult> GetWeeklyProgress(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var weeklyProgress = await userProgressRepository.GetByUserIdAndPeriodTypeAsync(userId, "weekly", ct);
        return Ok(weeklyProgress.Select(MapToDto));
    }

    [HttpGet("user/{userId:guid}/monthly")]
    public async Task<IActionResult> GetMonthlyProgress(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var monthlyProgress = await userProgressRepository.GetByUserIdAndPeriodTypeAsync(userId, "monthly", ct);
        return Ok(monthlyProgress.Select(MapToDto));
    }

    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var allProgress = await userProgressRepository.GetByUserIdAsync(userId, ct);
        var progressList = allProgress.ToList();

        var recordings = await recordingRepository.GetByUserIdAsync(userId, ct);
        var sessions = await speakingSessionRepository.GetByUserAsync(userId, ct);
        var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);

        var totalSessions = sessions.Count;
        var totalRecordings = recordings.Count;
        var totalPracticeMinutes = sessions
            .Where(s => s.TotalDurationSeconds.HasValue)
            .Sum(s => s.TotalDurationSeconds!.Value / 60);

        var scores = analyses
            .Where(a => a.OverallBandScore.HasValue)
            .Select(a => a.OverallBandScore!.Value)
            .ToList();

        var currentAvgScore = scores.Any() ? (decimal?)scores.Average() : null;
        var bestScore = scores.Any() ? (decimal?)scores.Max() : null;

        // Calculate improvement
        decimal? improvementPercentage = null;
        if (progressList.Count >= 2)
        {
            var recent = progressList.OrderByDescending(p => p.PeriodStart).Take(2).ToList();
            if (recent.Count == 2 && recent[0].AvgOverallScore.HasValue && recent[1].AvgOverallScore.HasValue)
            {
                var oldScore = recent[1].AvgOverallScore!.Value;
                var newScore = recent[0].AvgOverallScore!.Value;
                if (oldScore > 0)
                {
                    improvementPercentage = ((newScore - oldScore) / oldScore) * 100;
                }
            }
        }

        // Calculate streak (simplified - consecutive days with practice)
        var dailyProgress = progressList.Where(p => p.PeriodType == "daily" && p.TotalSessions > 0)
            .OrderByDescending(p => p.PeriodStart)
            .ToList();
        
        int currentStreak = 0;
        int longestStreak = 0;
        int tempStreak = 0;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        for (int i = 0; i < dailyProgress.Count; i++)
        {
            var expectedDate = today.AddDays(-i);
            var progress = dailyProgress.FirstOrDefault(p => p.PeriodStart == expectedDate);
            
            if (progress != null && progress.TotalSessions > 0)
            {
                if (i == 0) currentStreak++;
                tempStreak++;
                longestStreak = Math.Max(longestStreak, tempStreak);
            }
            else
            {
                if (i > 0) break; // Break if gap found
                tempStreak = 0;
            }
        }

        var statistics = new ProgressStatisticsDto
        {
            UserId = userId,
            TotalSessions = totalSessions,
            TotalRecordings = totalRecordings,
            TotalPracticeMinutes = totalPracticeMinutes,
            CurrentAvgScore = currentAvgScore,
            BestScore = bestScore,
            ImprovementPercentage = improvementPercentage,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak
        };

        return Ok(statistics);
    }

    [HttpGet("user/{userId:guid}/trends")]
    public async Task<IActionResult> GetTrends(
        Guid userId,
        [FromQuery] string periodType = "daily",
        [FromQuery] int periods = 30,
        CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var progress = await userProgressRepository.GetByUserIdAndPeriodTypeAsync(userId, periodType, ct);
        var trends = progress
            .OrderBy(p => p.PeriodStart)
            .TakeLast(periods)
            .Select(p => new
            {
                PeriodStart = p.PeriodStart,
                PeriodEnd = p.PeriodEnd,
                AvgOverallScore = p.AvgOverallScore,
                AvgFluencyScore = p.AvgFluencyScore,
                AvgVocabularyScore = p.AvgVocabularyScore,
                AvgGrammarScore = p.AvgGrammarScore,
                AvgPronunciationScore = p.AvgPronunciationScore,
                TotalSessions = p.TotalSessions,
                TotalRecordings = p.TotalRecordings,
                ScoreImprovement = p.ScoreImprovement
            })
            .ToList();

        return Ok(trends);
    }

    [HttpGet("user/{userId:guid}/improvement")]
    public async Task<IActionResult> GetImprovement(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var allProgress = await userProgressRepository.GetByUserIdAsync(userId, ct);
        var progressList = allProgress.OrderBy(p => p.PeriodStart).ToList();

        if (progressList.Count < 2)
        {
            return Ok(new
            {
                improvementPercentage = 0,
                scoreChange = 0,
                message = "Not enough data to calculate improvement"
            });
        }

        var first = progressList.First();
        var last = progressList.Last();

        decimal? improvementPercentage = null;
        decimal? scoreChange = null;

        if (first.AvgOverallScore.HasValue && last.AvgOverallScore.HasValue)
        {
            scoreChange = last.AvgOverallScore.Value - first.AvgOverallScore.Value;
            if (first.AvgOverallScore.Value > 0)
            {
                improvementPercentage = (scoreChange / first.AvgOverallScore.Value) * 100;
            }
        }

        return Ok(new
        {
            improvementPercentage,
            scoreChange,
            firstPeriod = new
            {
                PeriodStart = first.PeriodStart,
                AvgScore = first.AvgOverallScore
            },
            lastPeriod = new
            {
                PeriodStart = last.PeriodStart,
                AvgScore = last.AvgOverallScore
            }
        });
    }

    [HttpPost("user/{userId:guid}/calculate")]
    [Authorize(Roles = "Admin")]
    public Task<IActionResult> CalculateProgress(Guid userId, CancellationToken ct = default)
    {
        // This endpoint would trigger calculation of progress metrics
        // Implementation would aggregate data from sessions, recordings, and analyses
        // For now, return a placeholder response
        logger.LogInformation("Calculating progress for user {UserId}", userId);
        
        return Task.FromResult<IActionResult>(Ok(new { message = "Progress calculation initiated", userId }));
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static UserProgressDto MapToDto(Domain.Entities.UserProgress progress)
    {
        return new UserProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            PeriodType = progress.PeriodType,
            PeriodStart = progress.PeriodStart,
            PeriodEnd = progress.PeriodEnd,
            TotalSessions = progress.TotalSessions,
            TotalRecordings = progress.TotalRecordings,
            TotalPracticeMinutes = progress.TotalPracticeMinutes,
            AvgOverallScore = progress.AvgOverallScore,
            AvgFluencyScore = progress.AvgFluencyScore,
            AvgVocabularyScore = progress.AvgVocabularyScore,
            AvgGrammarScore = progress.AvgGrammarScore,
            AvgPronunciationScore = progress.AvgPronunciationScore,
            ScoreImprovement = progress.ScoreImprovement,
            ConsistencyScore = progress.ConsistencyScore,
            WeakestAreas = progress.WeakestAreas,
            StrongestAreas = progress.StrongestAreas,
            CreatedAt = progress.CreatedAt,
            UpdatedAt = progress.UpdatedAt
        };
    }
}
