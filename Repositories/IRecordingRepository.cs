using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IRecordingRepository
{
    Task AddAsync(Recording entity, CancellationToken ct);
    Task<Recording?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<Recording>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct);
    Task<IReadOnlyCollection<Recording>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyCollection<Recording>> GetByQuestionIdAsync(Guid questionId, CancellationToken ct);
    Task<IReadOnlyCollection<Recording>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task UpdateAsync(Recording entity, CancellationToken ct);
    Task DeleteAsync(Recording entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

