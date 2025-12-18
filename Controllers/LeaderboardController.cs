using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController(
    ApplicationDbContext context,
    ILogger<LeaderboardController> logger) : ControllerBase
{
    [HttpGet("top-scores")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopScores(
        [FromQuery] string period = "all", // all, week, month
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var cutoffDate = period switch
        {
            "week" => DateTimeOffset.UtcNow.AddDays(-7),
            "month" => DateTimeOffset.UtcNow.AddMonths(-1),
            _ => DateTimeOffset.MinValue
        };

        var topUsers = await context.AnalysisResults
            .Where(a => a.AnalyzedAt >= cutoffDate && a.OverallBandScore.HasValue)
            .GroupBy(a => a.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                AvgScore = g.Average(a => a.OverallBandScore!.Value),
                TotalRecordings = g.Count(),
                BestScore = g.Max(a => a.OverallBandScore!.Value)
            })
            .OrderByDescending(x => x.AvgScore)
            .Take(limit)
            .ToListAsync(ct);

        var leaderboard = new List<LeaderboardEntryDto>();
        int rank = 1;

        foreach (var entry in topUsers)
        {
            var user = await context.Users.FindAsync(new object[] { entry.UserId }, ct);
            if (user is not null)
            {
                leaderboard.Add(new LeaderboardEntryDto
                {
                    Rank = rank++,
                    UserId = entry.UserId,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    AvgScore = entry.AvgScore,
                    TotalRecordings = entry.TotalRecordings,
                    BestScore = entry.BestScore
                });
            }
        }

        return this.ApiOk(leaderboard, "Leaderboard retrieved successfully");
    }

    [HttpGet("top-streaks")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopStreaks(
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        // Get users with the longest current streaks (now using database column - MUCH FASTER!)
        var topStreaks = await context.Users
            .Where(u => u.IsActive && u.CurrentStreak > 0)
            .OrderByDescending(u => u.CurrentStreak)
            .ThenByDescending(u => u.LongestStreak)
            .Take(limit)
            .Select((u, index) => new StreakLeaderboardEntryDto
            {
                Rank = index + 1,
                UserId = u.Id,
                FullName = u.FullName,
                AvatarUrl = u.AvatarUrl,
                CurrentStreak = u.CurrentStreak,
                LongestStreak = u.LongestStreak,
                LastPracticeDate = u.LastPracticeDate
            })
            .ToListAsync(ct);

        // Fix rank numbering after query
        for (int i = 0; i < topStreaks.Count; i++)
        {
            topStreaks[i].Rank = i + 1;
        }

        return this.ApiOk(topStreaks, "Streak leaderboard retrieved successfully");
    }

    [HttpGet("top-practice-time")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopPracticeTime(
        [FromQuery] string period = "month", // week, month, all
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var cutoffDate = period switch
        {
            "week" => DateTimeOffset.UtcNow.AddDays(-7),
            "month" => DateTimeOffset.UtcNow.AddMonths(-1),
            _ => DateTimeOffset.MinValue
        };

        var topUsers = await context.PracticeSessions
            .Where(s => s.StartedAt >= cutoffDate && s.TotalDurationSeconds.HasValue)
            .GroupBy(s => s.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalMinutes = g.Sum(s => s.TotalDurationSeconds!.Value) / 60,
                TotalSessions = g.Count()
            })
            .OrderByDescending(x => x.TotalMinutes)
            .Take(limit)
            .ToListAsync(ct);

        var leaderboard = new List<PracticeTimeLeaderboardEntryDto>();
        int rank = 1;

        foreach (var entry in topUsers)
        {
            var user = await context.Users.FindAsync(new object[] { entry.UserId }, ct);
            if (user is not null)
            {
                leaderboard.Add(new PracticeTimeLeaderboardEntryDto
                {
                    Rank = rank++,
                    UserId = entry.UserId,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    TotalMinutes = entry.TotalMinutes,
                    TotalSessions = entry.TotalSessions
                });
            }
        }

        return this.ApiOk(leaderboard, "Practice time leaderboard retrieved successfully");
    }

    [HttpGet("my-rank")]
    [Authorize]
    public async Task<IActionResult> GetMyRank(
        [FromQuery] string category = "score", // score, streak, practice-time
        [FromQuery] string period = "all",
        CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        int rank = 0;
        object? stats = null;

        if (category == "score")
        {
            var cutoffDate = period switch
            {
                "week" => DateTimeOffset.UtcNow.AddDays(-7),
                "month" => DateTimeOffset.UtcNow.AddMonths(-1),
                _ => DateTimeOffset.MinValue
            };

            var userAvgScore = await context.AnalysisResults
                .Where(a => a.UserId == userId.Value && a.AnalyzedAt >= cutoffDate && a.OverallBandScore.HasValue)
                .AverageAsync(a => (decimal?)a.OverallBandScore, ct);

            if (userAvgScore.HasValue)
            {
                rank = await context.AnalysisResults
                    .Where(a => a.AnalyzedAt >= cutoffDate && a.OverallBandScore.HasValue)
                    .GroupBy(a => a.UserId)
                    .Select(g => g.Average(a => a.OverallBandScore!.Value))
                    .CountAsync(avg => avg > userAvgScore.Value, ct) + 1;

                stats = new { avgScore = userAvgScore.Value };
            }
        }
        else if (category == "streak")
        {
            // Calculate user's current streak
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var userProgress = await context.UserProgresses
                .Where(p => p.UserId == userId.Value && p.PeriodType == "daily" && p.TotalSessions > 0)
                .OrderByDescending(p => p.PeriodStart)
                .ToListAsync(ct);

            int currentStreak = 0;
            for (int i = 0; i < userProgress.Count; i++)
            {
                var expectedDate = today.AddDays(-i);
                var progress = userProgress.FirstOrDefault(p => p.PeriodStart == expectedDate);
                
                if (progress != null && progress.TotalSessions > 0)
                {
                    currentStreak++;
                }
                else
                {
                    break;
                }
            }

            // Count how many users have longer streaks (simplified)
            rank = 1; // Simplified - in production, calculate actual rank
            stats = new { currentStreak };
        }

        var user = await context.Users.FindAsync(new object[] { userId.Value }, ct);
        var response = new MyRankDto
        {
            UserId = userId.Value,
            FullName = user?.FullName ?? "",
            AvatarUrl = user?.AvatarUrl,
            Rank = rank,
            Category = category,
            Period = period,
            Stats = stats
        };

        return this.ApiOk(response, "Rank retrieved successfully");
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }
}

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public decimal AvgScore { get; set; }
    public int TotalRecordings { get; set; }
    public decimal BestScore { get; set; }
}

public class StreakLeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateOnly? LastPracticeDate { get; set; }
}

public class PracticeTimeLeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int TotalMinutes { get; set; }
    public int TotalSessions { get; set; }
}

public class MyRankDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int Rank { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public object? Stats { get; set; }
}


