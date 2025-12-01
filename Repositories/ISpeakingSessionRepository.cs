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
    Task UpdateAsync(PracticeSession entity, CancellationToken ct);
    Task DeleteAsync(PracticeSession entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

