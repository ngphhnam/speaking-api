using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Topic?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IEnumerable<Topic>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<IEnumerable<Topic>> GetByPartNumberAsync(int partNumber, CancellationToken ct = default);
    Task<IEnumerable<Topic>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<Topic> AddAsync(Topic topic, CancellationToken ct = default);
    Task UpdateAsync(Topic topic, CancellationToken ct = default);
    Task DeleteAsync(Topic topic, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}














