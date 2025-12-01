using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class UserVocabularyRepository(ApplicationDbContext context) : IUserVocabularyRepository
{
    public async Task AddAsync(UserVocabulary entity, CancellationToken ct)
        => await context.UserVocabularies.AddAsync(entity, ct);

    public Task<UserVocabulary?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .FirstOrDefaultAsync(uv => uv.Id == id, ct);

    public Task<UserVocabulary?> GetByUserAndVocabularyAsync(Guid userId, Guid vocabularyId, CancellationToken ct)
        => context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .FirstOrDefaultAsync(uv => uv.UserId == userId && uv.VocabularyId == vocabularyId, ct);

    public async Task<IEnumerable<UserVocabulary>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.UserVocabularies
            .AsNoTracking()
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId)
            .OrderByDescending(uv => uv.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<UserVocabulary>> GetByUserIdAndStatusAsync(Guid userId, string status, CancellationToken ct)
        => await context.UserVocabularies
            .AsNoTracking()
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId && uv.LearningStatus == status)
            .OrderByDescending(uv => uv.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<UserVocabulary>> GetDueForReviewAsync(Guid userId, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        return await context.UserVocabularies
            .AsNoTracking()
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId 
                && uv.LearningStatus != "mastered"
                && (uv.NextReviewAt == null || uv.NextReviewAt <= now))
            .OrderBy(uv => uv.NextReviewAt)
            .ToListAsync(ct);
    }

    public Task UpdateAsync(UserVocabulary entity, CancellationToken ct)
    {
        context.UserVocabularies.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserVocabulary entity, CancellationToken ct)
    {
        context.UserVocabularies.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

