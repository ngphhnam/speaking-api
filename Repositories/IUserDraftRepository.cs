using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IUserDraftRepository
{
    Task<UserDraft?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDraft?> GetByUserAndQuestionAsync(Guid userId, Guid questionId, CancellationToken ct = default);
    Task<UserDraft> AddAsync(UserDraft draft, CancellationToken ct = default);
    Task UpdateAsync(UserDraft draft, CancellationToken ct = default);
    Task DeleteAsync(UserDraft draft, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

