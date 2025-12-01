using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IAchievementRepository
{
    Task AddAsync(Achievement entity, CancellationToken ct);
    Task<Achievement?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Achievement>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<Achievement>> GetActiveAsync(CancellationToken ct);
    Task UpdateAsync(Achievement entity, CancellationToken ct);
    Task DeleteAsync(Achievement entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

