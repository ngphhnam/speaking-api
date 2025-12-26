using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class StreakHistoryRepository(ApplicationDbContext context) : IStreakHistoryRepository
{
    public async Task AddAsync(StreakHistory entity, CancellationToken ct)
        => await context.StreakHistories.AddAsync(entity, ct);

    public async Task<IReadOnlyCollection<StreakHistory>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.StreakHistories
            .AsNoTracking()
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.StartDate)
            .ToListAsync(ct);

    public Task<StreakHistory?> GetActiveStreakByUserIdAsync(Guid userId, CancellationToken ct)
        => context.StreakHistories
            .FirstOrDefaultAsync(sh => sh.UserId == userId && sh.IsActive, ct);

    public Task UpdateAsync(StreakHistory entity, CancellationToken ct)
    {
        context.StreakHistories.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}




