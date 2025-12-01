using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class UserDraftRepository(ApplicationDbContext context) : IUserDraftRepository
{
    public async Task<UserDraft?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Set<UserDraft>()
            .Include(d => d.Question)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<UserDraft?> GetByUserAndQuestionAsync(Guid userId, Guid questionId, CancellationToken ct = default)
    {
        return await context.Set<UserDraft>()
            .Include(d => d.Question)
            .FirstOrDefaultAsync(d => d.UserId == userId && d.QuestionId == questionId, ct);
    }

    public async Task<UserDraft> AddAsync(UserDraft draft, CancellationToken ct = default)
    {
        await context.Set<UserDraft>().AddAsync(draft, ct);
        return draft;
    }

    public Task UpdateAsync(UserDraft draft, CancellationToken ct = default)
    {
        context.Set<UserDraft>().Update(draft);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserDraft draft, CancellationToken ct = default)
    {
        context.Set<UserDraft>().Remove(draft);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}

