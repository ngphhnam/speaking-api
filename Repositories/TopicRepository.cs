using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class TopicRepository(ApplicationDbContext context) : ITopicRepository
{
    public async Task<Topic?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Topics
            .Include(t => t.Questions.Where(q => q.IsActive))
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<Topic?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await context.Topics
            .Include(t => t.Questions.Where(q => q.IsActive))
            .FirstOrDefaultAsync(t => t.Slug == slug, ct);
    }

    public async Task<IEnumerable<Topic>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var query = context.Topics.AsQueryable();
        
        if (!includeInactive)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query
            .Include(t => t.Questions.Where(q => q.IsActive))
            .OrderBy(t => t.PartNumber)
            .ThenBy(t => t.Title)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Topic>> GetByPartNumberAsync(int partNumber, CancellationToken ct = default)
    {
        return await context.Topics
            .Where(t => t.PartNumber == partNumber && t.IsActive)
            .Include(t => t.Questions.Where(q => q.IsActive))
            .OrderBy(t => t.Title)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Topic>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        return await context.Topics
            .Where(t => t.TopicCategory == category && t.IsActive)
            .Include(t => t.Questions.Where(q => q.IsActive))
            .OrderBy(t => t.Title)
            .ToListAsync(ct);
    }

    public async Task<Topic> AddAsync(Topic topic, CancellationToken ct = default)
    {
        await context.Topics.AddAsync(topic, ct);
        return topic;
    }

    public Task UpdateAsync(Topic topic, CancellationToken ct = default)
    {
        context.Topics.Update(topic);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Topic topic, CancellationToken ct = default)
    {
        context.Topics.Remove(topic);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}














