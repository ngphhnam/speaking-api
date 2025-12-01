using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IVocabularyRepository
{
    Task AddAsync(Vocabulary entity, CancellationToken ct);
    Task<Vocabulary?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Vocabulary?> GetByWordAsync(string word, CancellationToken ct);
    Task<IEnumerable<Vocabulary>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<Vocabulary>> SearchAsync(string query, CancellationToken ct);
    Task<IEnumerable<Vocabulary>> GetByBandLevelAsync(decimal? minBand, decimal? maxBand, CancellationToken ct);
    Task<IEnumerable<Vocabulary>> GetByTopicCategoryAsync(string category, CancellationToken ct);
    Task UpdateAsync(Vocabulary entity, CancellationToken ct);
    Task DeleteAsync(Vocabulary entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

