using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class SpeakingSessionRepository(ApplicationDbContext context) : ISpeakingSessionRepository
{
    public async Task AddAsync(PracticeSession entity, CancellationToken ct)
        => await context.PracticeSessions.AddAsync(entity, ct);

    public Task<PracticeSession?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.PracticeSessions.AsNoTracking()
            .Include(ps => ps.Recordings)
            .ThenInclude(r => r.AnalysisResult)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetByUserAsync(Guid userId, CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetActiveByUserAsync(Guid userId, CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == "in_progress")
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetCompletedByUserAsync(Guid userId, CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == "completed")
            .OrderByDescending(s => s.CompletedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetAllAsync(CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public Task UpdateAsync(PracticeSession entity, CancellationToken ct)
    {
        context.PracticeSessions.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PracticeSession entity, CancellationToken ct)
    {
        context.PracticeSessions.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

