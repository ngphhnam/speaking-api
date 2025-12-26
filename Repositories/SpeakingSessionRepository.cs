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
            .Include(ps => ps.Topic)
            .Include(ps => ps.Recordings)
            .ThenInclude(r => r.AnalysisResult)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetByUserAsync(Guid userId, CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Include(s => s.Topic)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetActiveByUserAsync(Guid userId, CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Include(s => s.Topic)
            .Where(s => s.UserId == userId && s.Status == "in_progress")
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetCompletedByUserAsync(Guid userId, CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Include(s => s.Topic)
            .Where(s => s.UserId == userId && s.Status == "completed")
            .OrderByDescending(s => s.CompletedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<PracticeSession>> GetAllAsync(CancellationToken ct)
        => await context.PracticeSessions.AsNoTracking()
            .Include(s => s.Topic)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<int> CountDailyPracticeSessionsAsync(Guid userId, DateOnly date, CancellationToken ct)
    {
        // Convert DateOnly to DateTimeOffset for proper comparison with CreatedAt (DateTimeOffset)
        var startOfDay = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var endOfDay = new DateTimeOffset(date.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
        
        return await context.PracticeSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId 
                && s.CreatedAt >= startOfDay 
                && s.CreatedAt <= endOfDay)
            .CountAsync(ct);
    }

    public async Task<int> CountPracticeSessionsInLast24HoursAsync(Guid userId, DateTimeOffset fromTime, CancellationToken ct)
    {
        var twentyFourHoursAgo = fromTime.AddHours(-24);
        
        return await context.PracticeSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId 
                && s.CreatedAt > twentyFourHoursAgo 
                && s.CreatedAt <= fromTime)
            .CountAsync(ct);
    }

    public Task<PracticeSession?> GetOldestSessionInLast24HoursAsync(Guid userId, DateTimeOffset fromTime, CancellationToken ct)
    {
        var twentyFourHoursAgo = fromTime.AddHours(-24);
        
        return context.PracticeSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId 
                && s.CreatedAt > twentyFourHoursAgo 
                && s.CreatedAt <= fromTime)
            .OrderBy(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

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

