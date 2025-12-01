using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Question>> GetByTopicIdAsync(Guid topicId, CancellationToken ct = default);
    Task<IEnumerable<Question>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<Question> AddAsync(Question question, CancellationToken ct = default);
    Task UpdateAsync(Question question, CancellationToken ct = default);
    Task DeleteAsync(Question question, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}














