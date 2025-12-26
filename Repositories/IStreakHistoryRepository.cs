using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IStreakHistoryRepository
{
    Task AddAsync(StreakHistory entity, CancellationToken ct);
    Task<IReadOnlyCollection<StreakHistory>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<StreakHistory?> GetActiveStreakByUserIdAsync(Guid userId, CancellationToken ct);
    Task UpdateAsync(StreakHistory entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

