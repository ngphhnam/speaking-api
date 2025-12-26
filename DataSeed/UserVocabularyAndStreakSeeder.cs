using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.DataSeed;

/// <summary>
/// Seeds UserVocabulary and StreakHistory data
/// </summary>
public static class UserVocabularyAndStreakSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var now = DateTimeOffset.UtcNow;

        // ============================================
        // STREAK HISTORY
        // ============================================
        // Check if streak history already exists
        if (!await context.StreakHistories.AnyAsync())
        {
            // Get users
            var users = await context.Users.ToListAsync();
            if (users.Any())
            {
                var streakHistories = new List<StreakHistory>();

                foreach (var user in users)
                {
                    // Tạo một số streak history cũ (đã kết thúc)
                    if (user.CurrentStreak > 0 || user.LongestStreak > 0)
                    {
                        // Streak hiện tại (nếu có)
                        if (user.CurrentStreak > 0 && user.LastPracticeDate.HasValue)
                        {
                            var activeStreak = new StreakHistory
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                StartDate = user.LastPracticeDate.Value.AddDays(-(user.CurrentStreak - 1)),
                                EndDate = null,
                                StreakLength = user.CurrentStreak,
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now
                            };
                            streakHistories.Add(activeStreak);
                        }

                        // Tạo một số streak cũ (đã kết thúc)
                        if (user.LongestStreak > user.CurrentStreak)
                        {
                            // Streak dài nhất trước đó
                            var pastStreak = new StreakHistory
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-60)),
                                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-40)),
                                StreakLength = user.LongestStreak,
                                IsActive = false,
                                CreatedAt = now.AddDays(-60),
                                UpdatedAt = now.AddDays(-40)
                            };
                            streakHistories.Add(pastStreak);
                        }

                        // Thêm một vài streak ngắn hơn
                        if (user.TotalPracticeDays > 10)
                        {
                            var shortStreak1 = new StreakHistory
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-25)),
                                StreakLength = 5,
                                IsActive = false,
                                CreatedAt = now.AddDays(-30),
                                UpdatedAt = now.AddDays(-25)
                            };
                            streakHistories.Add(shortStreak1);

                            var shortStreak2 = new StreakHistory
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)),
                                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
                                StreakLength = 5,
                                IsActive = false,
                                CreatedAt = now.AddDays(-20),
                                UpdatedAt = now.AddDays(-15)
                            };
                            streakHistories.Add(shortStreak2);
                        }
                    }
                }

                if (streakHistories.Any())
                {
                    await context.StreakHistories.AddRangeAsync(streakHistories);
                }
            }
        }

        // ============================================
        // USER VOCABULARY
        // ============================================
        // Check if user vocabulary already exists
        if (!await context.UserVocabularies.AnyAsync())
        {
            // Get users and vocabularies
            var users = await context.Users.ToListAsync();
            var vocabularies = await context.Vocabularies.Take(20).ToListAsync(); // Lấy 20 từ đầu tiên

            if (users.Any() && vocabularies.Any())
            {
                var userVocabularies = new List<UserVocabulary>();
                var random = new Random();

                foreach (var user in users)
                {
                    // Mỗi user sẽ có một số từ vựng (random 5-15 từ)
                    var vocabCount = random.Next(5, Math.Min(16, vocabularies.Count));
                    var selectedVocabs = vocabularies.OrderBy(x => random.Next()).Take(vocabCount).ToList();

                    foreach (var vocab in selectedVocabs)
                    {
                        var learningStatus = random.Next(0, 3) switch
                        {
                            0 => "learning",
                            1 => "reviewing",
                            _ => "mastered"
                        };

                        var firstEncountered = DateTimeOffset.UtcNow.AddDays(-random.Next(1, 30));
                        DateTimeOffset? nextReviewAt = learningStatus == "mastered" 
                            ? null 
                            : DateTimeOffset.UtcNow.AddDays(random.Next(1, 7));

                        var userVocab = new UserVocabulary
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            VocabularyId = vocab.Id,
                            LearningStatus = learningStatus,
                            NextReviewAt = nextReviewAt,
                            ReviewCount = learningStatus == "mastered" ? random.Next(5, 15) : random.Next(0, 5),
                            SuccessCount = learningStatus == "mastered" ? random.Next(5, 10) : random.Next(0, 5),
                            PersonalNotes = random.Next(0, 3) == 0 ? $"Note for {vocab.Word}" : null,
                            ExampleUsage = random.Next(0, 2) == 0 ? $"I used {vocab.Word} in my practice." : null,
                            FirstEncounteredAt = firstEncountered,
                            LastReviewedAt = learningStatus != "learning" 
                                ? DateTimeOffset.UtcNow.AddDays(-random.Next(1, 7)) 
                                : null,
                            MasteredAt = learningStatus == "mastered" 
                                ? DateTimeOffset.UtcNow.AddDays(-random.Next(1, 10)) 
                                : null,
                            CreatedAt = firstEncountered
                        };

                        userVocabularies.Add(userVocab);
                    }
                }

                if (userVocabularies.Any())
                {
                    await context.UserVocabularies.AddRangeAsync(userVocabularies);
                }
            }
        }

        await context.SaveChangesAsync();
    }
}

