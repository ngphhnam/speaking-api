using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IUserAchievementRepository
{
    Task AddAsync(UserAchievement entity, CancellationToken ct);
    Task<UserAchievement?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserAchievement?> GetByUserAndAchievementAsync(Guid userId, Guid achievementId, CancellationToken ct);
    Task<IEnumerable<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IEnumerable<UserAchievement>> GetCompletedByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IEnumerable<UserAchievement>> GetInProgressByUserIdAsync(Guid userId, CancellationToken ct);
    Task UpdateAsync(UserAchievement entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

