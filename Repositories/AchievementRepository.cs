using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class AchievementRepository(ApplicationDbContext context) : IAchievementRepository
{
    public async Task AddAsync(Achievement entity, CancellationToken ct)
        => await context.Achievements.AddAsync(entity, ct);

    public Task<Achievement?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Achievements
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<Achievement>> GetAllAsync(CancellationToken ct)
        => await context.Achievements
            .AsNoTracking()
            .OrderBy(a => a.Title)
            .ToListAsync(ct);

    public async Task<IEnumerable<Achievement>> GetActiveAsync(CancellationToken ct)
        => await context.Achievements
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Title)
            .ToListAsync(ct);

    public Task UpdateAsync(Achievement entity, CancellationToken ct)
    {
        context.Achievements.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Achievement entity, CancellationToken ct)
    {
        context.Achievements.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

