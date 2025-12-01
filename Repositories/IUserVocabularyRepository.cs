using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IUserVocabularyRepository
{
    Task AddAsync(UserVocabulary entity, CancellationToken ct);
    Task<UserVocabulary?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserVocabulary?> GetByUserAndVocabularyAsync(Guid userId, Guid vocabularyId, CancellationToken ct);
    Task<IEnumerable<UserVocabulary>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IEnumerable<UserVocabulary>> GetByUserIdAndStatusAsync(Guid userId, string status, CancellationToken ct);
    Task<IEnumerable<UserVocabulary>> GetDueForReviewAsync(Guid userId, CancellationToken ct);
    Task UpdateAsync(UserVocabulary entity, CancellationToken ct);
    Task DeleteAsync(UserVocabulary entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

