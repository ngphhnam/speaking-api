using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;
using System.Text.Json;

namespace SpeakingPractice.Api.Services;

public class AchievementService(
    ApplicationDbContext context,
    IAchievementRepository achievementRepository,
    IUserAchievementRepository userAchievementRepository,
    UserManager<ApplicationUser> userManager,
    IStreakService streakService,
    ILogger<AchievementService> logger) : IAchievementService
{
    // Công thức tính XP cần để lên level: XP = 100 * level^1.5
    private static int CalculateXpForLevel(int level) => (int)(100 * Math.Pow(level, 1.5));
    
    public async Task<List<AchievementAwardedDto>> CheckAndAwardAchievementsAsync(
        Guid userId, 
        decimal? bandScore = null,
        CancellationToken ct = default)
    {
        var awardedAchievements = new List<AchievementAwardedDto>();
        
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return awardedAchievements;

        // Lấy thông tin streak
        var streakInfo = await streakService.GetStreakInfoAsync(userId, ct);
        
        // Lấy tất cả achievements đang active
        var achievements = await achievementRepository.GetActiveAsync(ct);
        
        foreach (var achievement in achievements)
        {
            // Kiểm tra xem user đã có achievement này chưa
            var userAchievement = await userAchievementRepository.GetByUserAndAchievementAsync(userId, achievement.Id, ct);
            
            if (userAchievement != null && userAchievement.IsCompleted)
            {
                continue; // Đã có achievement này rồi
            }

            bool shouldAward = false;
            
            // Kiểm tra theo loại achievement
            switch (achievement.AchievementType.ToLowerInvariant())
            {
                case "practice_streak":
                    shouldAward = CheckStreakAchievement(achievement, streakInfo);
                    break;
                case "score_milestone":
                    shouldAward = CheckScoreAchievement(achievement, bandScore);
                    break;
                case "total_practice_days":
                    shouldAward = CheckTotalPracticeDaysAchievement(achievement, streakInfo);
                    break;
                case "total_questions":
                    shouldAward = await CheckTotalQuestionsAchievement(achievement, userId, ct);
                    break;
            }

            if (shouldAward)
            {
                // Trao achievement
                if (userAchievement == null)
                {
                    userAchievement = new UserAchievement
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        AchievementId = achievement.Id,
                        IsCompleted = true,
                        EarnedAt = DateTimeOffset.UtcNow,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    await userAchievementRepository.AddAsync(userAchievement, ct);
                }
                else
                {
                    userAchievement.IsCompleted = true;
                    userAchievement.EarnedAt = DateTimeOffset.UtcNow;
                    await userAchievementRepository.UpdateAsync(userAchievement, ct);
                }

                // Cập nhật điểm và level
                var levelResult = await UpdateUserLevelAsync(userId, achievement.Points, ct);
                
                awardedAchievements.Add(new AchievementAwardedDto
                {
                    AchievementId = achievement.Id,
                    Title = achievement.Title,
                    BadgeIconUrl = achievement.BadgeIconUrl,
                    Points = achievement.Points,
                    IsNewLevel = levelResult.LeveledUp,
                    NewLevel = levelResult.LeveledUp ? levelResult.NewLevel : null
                });

                logger.LogInformation(
                    "User {UserId} earned achievement: {AchievementTitle} ({Points} points)",
                    userId,
                    achievement.Title,
                    achievement.Points);
            }
        }

        if (awardedAchievements.Count > 0)
        {
            await userAchievementRepository.SaveChangesAsync(ct);
        }

        return awardedAchievements;
    }

    public async Task<LevelUpdateResult> UpdateUserLevelAsync(Guid userId, int pointsEarned, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException($"User with id {userId} not found");
        }

        var oldLevel = user.Level;
        var oldXp = user.ExperiencePoints;
        
        user.ExperiencePoints += pointsEarned;
        user.TotalPoints += pointsEarned;
        
        // Tính level mới
        int newLevel = oldLevel;
        while (user.ExperiencePoints >= CalculateXpForLevel(newLevel + 1))
        {
            newLevel++;
        }
        
        user.Level = newLevel;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        
        await userManager.UpdateAsync(user);
        
        var pointsToNextLevel = newLevel < 100 
            ? CalculateXpForLevel(newLevel + 1) - user.ExperiencePoints 
            : 0;

        return new LevelUpdateResult
        {
            OldLevel = oldLevel,
            NewLevel = newLevel,
            OldExperiencePoints = oldXp,
            NewExperiencePoints = user.ExperiencePoints,
            LeveledUp = newLevel > oldLevel,
            PointsToNextLevel = pointsToNextLevel
        };
    }

    public async Task<UserLevelInfo?> GetUserLevelInfoAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        var currentLevelXp = CalculateXpForLevel(user.Level);
        var nextLevelXp = user.Level < 100 ? CalculateXpForLevel(user.Level + 1) : user.ExperiencePoints;
        var pointsToNextLevel = nextLevelXp - user.ExperiencePoints;

        return new UserLevelInfo
        {
            Level = user.Level,
            ExperiencePoints = user.ExperiencePoints,
            TotalPoints = user.TotalPoints,
            PointsToNextLevel = Math.Max(0, pointsToNextLevel),
            PointsForCurrentLevel = currentLevelXp
        };
    }

    private bool CheckStreakAchievement(Achievement achievement, StreakInfo? streakInfo)
    {
        if (streakInfo == null) return false;
        
        var criteria = JsonSerializer.Deserialize<Dictionary<string, object>>(achievement.RequirementCriteria);
        if (criteria == null || !criteria.ContainsKey("streak_days")) return false;
        
        var requiredStreak = Convert.ToInt32(criteria["streak_days"].ToString());
        return streakInfo.CurrentStreak >= requiredStreak;
    }

    private bool CheckScoreAchievement(Achievement achievement, decimal? bandScore)
    {
        if (!bandScore.HasValue) return false;
        
        var criteria = JsonSerializer.Deserialize<Dictionary<string, object>>(achievement.RequirementCriteria);
        if (criteria == null || !criteria.ContainsKey("min_score")) return false;
        
        var minScore = Convert.ToDecimal(criteria["min_score"].ToString());
        return bandScore.Value >= minScore;
    }

    private bool CheckTotalPracticeDaysAchievement(Achievement achievement, StreakInfo? streakInfo)
    {
        if (streakInfo == null) return false;
        
        var criteria = JsonSerializer.Deserialize<Dictionary<string, object>>(achievement.RequirementCriteria);
        if (criteria == null || !criteria.ContainsKey("total_days")) return false;
        
        var requiredDays = Convert.ToInt32(criteria["total_days"].ToString());
        return streakInfo.TotalPracticeDays >= requiredDays;
    }

    private async Task<bool> CheckTotalQuestionsAchievement(Achievement achievement, Guid userId, CancellationToken ct)
    {
        var criteria = JsonSerializer.Deserialize<Dictionary<string, object>>(achievement.RequirementCriteria);
        if (criteria == null || !criteria.ContainsKey("total_questions")) return false;
        
        var requiredQuestions = Convert.ToInt32(criteria["total_questions"].ToString());
        
        // Đếm số câu hỏi đã hoàn thành
        var completedCount = await context.Recordings
            .Where(r => r.UserId == userId && r.ProcessingStatus == "completed")
            .CountAsync(ct);
        
        return completedCount >= requiredQuestions;
    }
}

