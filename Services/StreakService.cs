using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class StreakService(
    ApplicationDbContext context,
    ILogger<StreakService> logger) : IStreakService
{
    public async Task<StreakUpdateResult> UpdateStreakAsync(
        Guid userId, 
        DateOnly? practiceDate = null, 
        CancellationToken ct = default)
    {
        var today = practiceDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        
        var user = await context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(ct);
        
        if (user is null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }

        var lastPractice = user.LastPracticeDate;
        var currentStreak = user.CurrentStreak;
        var longestStreak = user.LongestStreak;
        var totalDays = user.TotalPracticeDays;

        var result = new StreakUpdateResult();

        if (lastPractice is null)
        {
            // First time practicing
            user.CurrentStreak = 1;
            user.LongestStreak = 1;
            user.TotalPracticeDays = 1;
            user.LastPracticeDate = today;
            
            result.CurrentStreak = 1;
            result.LongestStreak = 1;
            result.TotalPracticeDays = 1;
            result.IsNewRecord = true;
            result.StreakContinued = true;
            
            logger.LogInformation("User {UserId} started their first practice streak", userId);
        }
        else
        {
            var daysDifference = today.DayNumber - lastPractice.Value.DayNumber;
            
            if (daysDifference == 0)
            {
                // Practicing again on the same day - no streak change
                result.CurrentStreak = user.CurrentStreak;
                result.LongestStreak = user.LongestStreak;
                result.TotalPracticeDays = user.TotalPracticeDays;
                result.StreakContinued = true;
                
                logger.LogDebug("User {UserId} practiced again on the same day", userId);
            }
            else if (daysDifference == 1)
            {
                // Consecutive day - increase streak
                user.CurrentStreak += 1;
                user.TotalPracticeDays += 1;
                user.LastPracticeDate = today;
                
                if (user.CurrentStreak > user.LongestStreak)
                {
                    user.LongestStreak = user.CurrentStreak;
                    result.IsNewRecord = true;
                    logger.LogInformation(
                        "User {UserId} achieved new longest streak: {Streak} days!", 
                        userId, 
                        user.CurrentStreak);
                }
                
                result.CurrentStreak = user.CurrentStreak;
                result.LongestStreak = user.LongestStreak;
                result.TotalPracticeDays = user.TotalPracticeDays;
                result.StreakContinued = true;
                
                logger.LogInformation(
                    "User {UserId} continued streak: {Streak} days", 
                    userId, 
                    user.CurrentStreak);
            }
            else
            {
                // Streak broken - reset to 1
                logger.LogInformation(
                    "User {UserId} broke their {OldStreak}-day streak after {DaysMissed} days",
                    userId,
                    user.CurrentStreak,
                    daysDifference - 1);
                
                user.CurrentStreak = 1;
                user.TotalPracticeDays += 1;
                user.LastPracticeDate = today;
                
                result.CurrentStreak = 1;
                result.LongestStreak = user.LongestStreak;
                result.TotalPracticeDays = user.TotalPracticeDays;
                result.StreakBroken = true;
            }
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(ct);

        return result;
    }

    public async Task<StreakInfo?> GetStreakInfoAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new StreakInfo
            {
                UserId = u.Id,
                CurrentStreak = u.CurrentStreak,
                LongestStreak = u.LongestStreak,
                LastPracticeDate = u.LastPracticeDate,
                TotalPracticeDays = u.TotalPracticeDays
            })
            .FirstOrDefaultAsync(ct);

        return user;
    }

    public async Task<int> ResetExpiredStreaksAsync(CancellationToken ct = default)
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        
        // Find users whose last practice was more than 1 day ago and still have a streak
        var usersToReset = await context.Users
            .Where(u => u.CurrentStreak > 0 
                     && u.LastPracticeDate.HasValue 
                     && u.LastPracticeDate < yesterday)
            .ToListAsync(ct);

        foreach (var user in usersToReset)
        {
            logger.LogInformation(
                "Resetting expired streak for user {UserId}: {Streak} days (last practice: {LastDate})",
                user.Id,
                user.CurrentStreak,
                user.LastPracticeDate);
            
            user.CurrentStreak = 0;
            user.UpdatedAt = DateTimeOffset.UtcNow;
        }

        if (usersToReset.Count > 0)
        {
            await context.SaveChangesAsync(ct);
        }

        return usersToReset.Count;
    }
}






