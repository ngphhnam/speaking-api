using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class UserAchievementRepository(ApplicationDbContext context) : IUserAchievementRepository
{
    public async Task AddAsync(UserAchievement entity, CancellationToken ct)
        => await context.UserAchievements.AddAsync(entity, ct);

    public Task<UserAchievement?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.UserAchievements
            .Include(ua => ua.Achievement)
            .FirstOrDefaultAsync(ua => ua.Id == id, ct);

    public Task<UserAchievement?> GetByUserAndAchievementAsync(Guid userId, Guid achievementId, CancellationToken ct)
        => context.UserAchievements
            .Include(ua => ua.Achievement)
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId, ct);

    public async Task<IEnumerable<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.UserAchievements
            .AsNoTracking()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.EarnedAt)
            .ThenBy(ua => ua.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<UserAchievement>> GetCompletedByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.UserAchievements
            .AsNoTracking()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId && ua.IsCompleted)
            .OrderByDescending(ua => ua.EarnedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<UserAchievement>> GetInProgressByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.UserAchievements
            .AsNoTracking()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId && !ua.IsCompleted)
            .OrderByDescending(ua => ua.CreatedAt)
            .ToListAsync(ct);

    public Task UpdateAsync(UserAchievement entity, CancellationToken ct)
    {
        context.UserAchievements.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

