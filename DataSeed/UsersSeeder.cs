using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.DataSeed;

/// <summary>
/// Seeds the database with sample user data for development and testing
/// </summary>
public static class UsersSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        // Check if users already exist
        if (await userManager.Users.AnyAsync())
        {
            return; // Data already seeded
        }

        var now = DateTimeOffset.UtcNow;

        // ============================================
        // CREATE ROLES FIRST
        // ============================================
        var roles = new[] { "Admin", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        // ============================================
        // ADMIN USERS
        // ============================================
        var adminUser = new ApplicationUser
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = "admin@speakingpractice.com",
            UserName = "admin@speakingpractice.com",
            FullName = "Admin User",
            EmailVerified = true,
            IsActive = true,
            SubscriptionType = "premium",
            CurrentLevel = "advanced",
            Level = 10,
            ExperiencePoints = 5000,
            TotalPoints = 5000,
            CreatedAt = now,
            UpdatedAt = now
        };

        var adminResult = await userManager.CreateAsync(adminUser, "Admin@123456");
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // ============================================
        // REGULAR USERS (Sample)
        // ============================================
        var regularUsers = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Email = "user1@test.com",
                UserName = "user1@test.com",
                FullName = "User One",
                EmailVerified = true,
                IsActive = true,
                SubscriptionType = "premium",
                SubscriptionExpiresAt = DateTime.UtcNow.AddYears(1),
                CurrentLevel = "intermediate",
                TargetBandScore = 7.5m,
                Level = 5,
                ExperiencePoints = 2000,
                TotalPoints = 2000,
                CurrentStreak = 15,
                LongestStreak = 30,
                TotalPracticeDays = 45,
                LastPracticeDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = now,
                UpdatedAt = now
            },
            new ApplicationUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Email = "user2@test.com",
                UserName = "user2@test.com",
                FullName = "User Two",
                EmailVerified = true,
                IsActive = true,
                SubscriptionType = "free",
                CurrentLevel = "beginner",
                TargetBandScore = 6.0m,
                Level = 1,
                ExperiencePoints = 50,
                TotalPoints = 50,
                CurrentStreak = 3,
                LongestStreak = 5,
                TotalPracticeDays = 10,
                LastPracticeDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = now,
                UpdatedAt = now
            },
            new ApplicationUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Email = "user3@test.com",
                UserName = "user3@test.com",
                FullName = "User Three",
                EmailVerified = true,
                IsActive = true,
                SubscriptionType = "free",
                CurrentLevel = "intermediate",
                TargetBandScore = 7.0m,
                Level = 3,
                ExperiencePoints = 500,
                TotalPoints = 500,
                CurrentStreak = 0,
                LongestStreak = 10,
                TotalPracticeDays = 20,
                LastPracticeDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
                CreatedAt = now,
                UpdatedAt = now
            },
            new ApplicationUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Email = "user4@test.com",
                UserName = "user4@test.com",
                FullName = "User Four",
                EmailVerified = false,
                IsActive = true,
                SubscriptionType = "free",
                CurrentLevel = "beginner",
                Level = 1,
                ExperiencePoints = 0,
                TotalPoints = 0,
                CurrentStreak = 0,
                LongestStreak = 0,
                TotalPracticeDays = 0,
                CreatedAt = now,
                UpdatedAt = now
            },
            new ApplicationUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                Email = "user5@test.com",
                UserName = "user5@test.com",
                FullName = "User Five",
                EmailVerified = true,
                IsActive = true,
                SubscriptionType = "premium",
                SubscriptionExpiresAt = DateTime.UtcNow.AddMonths(6),
                CurrentLevel = "advanced",
                TargetBandScore = 8.5m,
                Level = 8,
                ExperiencePoints = 4000,
                TotalPoints = 4000,
                CurrentStreak = 20,
                LongestStreak = 20,
                TotalPracticeDays = 50,
                LastPracticeDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        foreach (var user in regularUsers)
        {
            var result = await userManager.CreateAsync(user, "User@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}

