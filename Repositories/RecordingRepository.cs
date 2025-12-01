using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class RecordingRepository(ApplicationDbContext context) : IRecordingRepository
{
    public async Task AddAsync(Recording entity, CancellationToken ct)
        => await context.Recordings.AddAsync(entity, ct);

    public Task<Recording?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Recordings
            .Include(r => r.AnalysisResult)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyCollection<Recording>> GetBySessionIdAsync(Guid sessionId, CancellationToken ct)
        => await context.Recordings.AsNoTracking()
            .Where(r => r.SessionId == sessionId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<Recording>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.Recordings.AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<Recording>> GetByQuestionIdAsync(Guid questionId, CancellationToken ct)
        => await context.Recordings.AsNoTracking()
            .Where(r => r.QuestionId == questionId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<Recording>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        => await context.Recordings.AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task UpdateAsync(Recording entity, CancellationToken ct)
    {
        context.Recordings.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Recording entity, CancellationToken ct)
    {
        context.Recordings.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

