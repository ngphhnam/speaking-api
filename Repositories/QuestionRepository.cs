using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class QuestionRepository(ApplicationDbContext context) : IQuestionRepository
{
    public async Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Questions
            .Include(q => q.Topic)
            .FirstOrDefaultAsync(q => q.Id == id, ct);
    }

    public async Task<IEnumerable<Question>> GetByTopicIdAsync(Guid topicId, CancellationToken ct = default)
    {
        return await context.Questions
            .Where(q => q.TopicId == topicId && q.IsActive)
            .Include(q => q.Topic)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Question>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var query = context.Questions.AsQueryable();
        
        if (!includeInactive)
        {
            query = query.Where(q => q.IsActive);
        }

        return await query
            .Include(q => q.Topic)
            .OrderBy(q => q.TopicId)
            .ThenBy(q => q.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Question> AddAsync(Question question, CancellationToken ct = default)
    {
        await context.Questions.AddAsync(question, ct);
        return question;
    }

    public Task UpdateAsync(Question question, CancellationToken ct = default)
    {
        context.Questions.Update(question);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Question question, CancellationToken ct = default)
    {
        context.Questions.Remove(question);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}














