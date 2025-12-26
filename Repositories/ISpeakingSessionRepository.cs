using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface ISpeakingSessionRepository
{
    Task AddAsync(PracticeSession entity, CancellationToken ct);
    Task<PracticeSession?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<PracticeSession>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyCollection<PracticeSession>> GetActiveByUserAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyCollection<PracticeSession>> GetCompletedByUserAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyCollection<PracticeSession>> GetAllAsync(CancellationToken ct);
    Task<int> CountDailyPracticeSessionsAsync(Guid userId, DateOnly date, CancellationToken ct);
    Task<int> CountPracticeSessionsInLast24HoursAsync(Guid userId, DateTimeOffset fromTime, CancellationToken ct);
    Task<PracticeSession?> GetOldestSessionInLast24HoursAsync(Guid userId, DateTimeOffset fromTime, CancellationToken ct);
    Task UpdateAsync(PracticeSession entity, CancellationToken ct);
    Task DeleteAsync(PracticeSession entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

