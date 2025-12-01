using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class UserProgressRepository(ApplicationDbContext context) : IUserProgressRepository
{
    public async Task AddAsync(UserProgress entity, CancellationToken ct)
        => await context.UserProgresses.AddAsync(entity, ct);

    public Task<UserProgress?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.UserProgresses
            .FirstOrDefaultAsync(up => up.Id == id, ct);

    public Task<UserProgress?> GetByUserAndPeriodAsync(Guid userId, string periodType, DateOnly periodStart, CancellationToken ct)
        => context.UserProgresses
            .FirstOrDefaultAsync(up => up.UserId == userId 
                && up.PeriodType == periodType 
                && up.PeriodStart == periodStart, ct);

    public async Task<IEnumerable<UserProgress>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.UserProgresses
            .AsNoTracking()
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.PeriodStart)
            .ToListAsync(ct);

    public async Task<IEnumerable<UserProgress>> GetByUserIdAndPeriodTypeAsync(Guid userId, string periodType, CancellationToken ct)
        => await context.UserProgresses
            .AsNoTracking()
            .Where(up => up.UserId == userId && up.PeriodType == periodType)
            .OrderByDescending(up => up.PeriodStart)
            .ToListAsync(ct);

    public Task UpdateAsync(UserProgress entity, CancellationToken ct)
    {
        context.UserProgresses.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

