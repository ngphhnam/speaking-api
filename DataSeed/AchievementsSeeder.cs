using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SpeakingPractice.Api.DataSeed;

/// <summary>
/// Seeds the database with achievement data
/// </summary>
public static class AchievementsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if achievements already exist
        if (await context.Achievements.AnyAsync())
        {
            return; // Data already seeded
        }

        var now = DateTimeOffset.UtcNow;

        var achievements = new List<Achievement>();

        // ============================================
        // 1. PRACTICE STREAK ACHIEVEMENTS
        // ============================================
        achievements.Add(new Achievement
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Title = "First Flame",
            Description = "Luyện tập 3 ngày liên tiếp",
            AchievementType = "practice_streak",
            RequirementCriteria = """{"streak_days": 3}""",
            Points = 50,
            BadgeIconUrl = "/badges/first-flame.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Title = "Week Warrior",
            Description = "Luyện tập 7 ngày liên tiếp",
            AchievementType = "practice_streak",
            RequirementCriteria = """{"streak_days": 7}""",
            Points = 150,
            BadgeIconUrl = "/badges/week-warrior.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Title = "Fortnight Fighter",
            Description = "Luyện tập 14 ngày liên tiếp",
            AchievementType = "practice_streak",
            RequirementCriteria = """{"streak_days": 14}""",
            Points = 300,
            BadgeIconUrl = "/badges/fortnight-fighter.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Title = "Monthly Master",
            Description = "Luyện tập 30 ngày liên tiếp",
            AchievementType = "practice_streak",
            RequirementCriteria = """{"streak_days": 30}""",
            Points = 500,
            BadgeIconUrl = "/badges/monthly-master.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Title = "Century Champion",
            Description = "Luyện tập 100 ngày liên tiếp",
            AchievementType = "practice_streak",
            RequirementCriteria = """{"streak_days": 100}""",
            Points = 1000,
            BadgeIconUrl = "/badges/century-champion.png",
            IsActive = true,
            CreatedAt = now
        });

        // ============================================
        // 2. SCORE MILESTONE ACHIEVEMENTS
        // ============================================
        achievements.Add(new Achievement
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            Title = "Getting Started",
            Description = "Đạt điểm 5.0",
            AchievementType = "score_milestone",
            RequirementCriteria = """{"min_score": 5.0}""",
            Points = 30,
            BadgeIconUrl = "/badges/getting-started.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            Title = "Good Progress",
            Description = "Đạt điểm 6.0",
            AchievementType = "score_milestone",
            RequirementCriteria = """{"min_score": 6.0}""",
            Points = 50,
            BadgeIconUrl = "/badges/good-progress.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            Title = "Great Job",
            Description = "Đạt điểm 7.0",
            AchievementType = "score_milestone",
            RequirementCriteria = """{"min_score": 7.0}""",
            Points = 100,
            BadgeIconUrl = "/badges/great-job.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            Title = "Excellent",
            Description = "Đạt điểm 8.0",
            AchievementType = "score_milestone",
            RequirementCriteria = """{"min_score": 8.0}""",
            Points = 200,
            BadgeIconUrl = "/badges/excellent.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Title = "Perfect Score",
            Description = "Đạt điểm 9.0",
            AchievementType = "score_milestone",
            RequirementCriteria = """{"min_score": 9.0}""",
            Points = 500,
            BadgeIconUrl = "/badges/perfect-score.png",
            IsActive = true,
            CreatedAt = now
        });

        // ============================================
        // 3. TOTAL PRACTICE DAYS ACHIEVEMENTS
        // ============================================
        achievements.Add(new Achievement
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Title = "Week Explorer",
            Description = "Luyện tập tổng cộng 7 ngày",
            AchievementType = "total_practice_days",
            RequirementCriteria = """{"total_days": 7}""",
            Points = 50,
            BadgeIconUrl = "/badges/week-explorer.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Title = "Month Explorer",
            Description = "Luyện tập tổng cộng 30 ngày",
            AchievementType = "total_practice_days",
            RequirementCriteria = """{"total_days": 30}""",
            Points = 150,
            BadgeIconUrl = "/badges/month-explorer.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            Title = "Quarter Explorer",
            Description = "Luyện tập tổng cộng 90 ngày",
            AchievementType = "total_practice_days",
            RequirementCriteria = """{"total_days": 90}""",
            Points = 300,
            BadgeIconUrl = "/badges/quarter-explorer.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            Title = "Year Explorer",
            Description = "Luyện tập tổng cộng 365 ngày",
            AchievementType = "total_practice_days",
            RequirementCriteria = """{"total_days": 365}""",
            Points = 1000,
            BadgeIconUrl = "/badges/year-explorer.png",
            IsActive = true,
            CreatedAt = now
        });

        // ============================================
        // 4. TOTAL QUESTIONS ACHIEVEMENTS
        // ============================================
        achievements.Add(new Achievement
        {
            Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            Title = "First Steps",
            Description = "Hoàn thành 10 câu hỏi",
            AchievementType = "total_questions",
            RequirementCriteria = """{"total_questions": 10}""",
            Points = 30,
            BadgeIconUrl = "/badges/first-steps.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("10101010-1010-1010-1010-101010101010"),
            Title = "Getting Serious",
            Description = "Hoàn thành 50 câu hỏi",
            AchievementType = "total_questions",
            RequirementCriteria = """{"total_questions": 50}""",
            Points = 100,
            BadgeIconUrl = "/badges/getting-serious.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("20202020-2020-2020-2020-202020202020"),
            Title = "Dedicated Learner",
            Description = "Hoàn thành 100 câu hỏi",
            AchievementType = "total_questions",
            RequirementCriteria = """{"total_questions": 100}""",
            Points = 200,
            BadgeIconUrl = "/badges/dedicated-learner.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("30303030-3030-3030-3030-303030303030"),
            Title = "Practice Master",
            Description = "Hoàn thành 500 câu hỏi",
            AchievementType = "total_questions",
            RequirementCriteria = """{"total_questions": 500}""",
            Points = 500,
            BadgeIconUrl = "/badges/practice-master.png",
            IsActive = true,
            CreatedAt = now
        });

        achievements.Add(new Achievement
        {
            Id = Guid.Parse("40404040-4040-4040-4040-404040404040"),
            Title = "Question King",
            Description = "Hoàn thành 1000 câu hỏi",
            AchievementType = "total_questions",
            RequirementCriteria = """{"total_questions": 1000}""",
            Points = 1000,
            BadgeIconUrl = "/badges/question-king.png",
            IsActive = true,
            CreatedAt = now
        });

        // Add all achievements to context
        await context.Achievements.AddRangeAsync(achievements);
        await context.SaveChangesAsync();
    }
}

