using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IAnalysisResultRepository
{
    Task AddAsync(AnalysisResult entity, CancellationToken ct);
    Task<AnalysisResult?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AnalysisResult?> GetByRecordingIdAsync(Guid recordingId, CancellationToken ct);
    Task<IEnumerable<AnalysisResult>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task UpdateAsync(AnalysisResult entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

