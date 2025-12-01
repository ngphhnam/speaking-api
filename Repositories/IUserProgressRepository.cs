using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IUserProgressRepository
{
    Task AddAsync(UserProgress entity, CancellationToken ct);
    Task<UserProgress?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserProgress?> GetByUserAndPeriodAsync(Guid userId, string periodType, DateOnly periodStart, CancellationToken ct);
    Task<IEnumerable<UserProgress>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IEnumerable<UserProgress>> GetByUserIdAndPeriodTypeAsync(Guid userId, string periodType, CancellationToken ct);
    Task UpdateAsync(UserProgress entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

