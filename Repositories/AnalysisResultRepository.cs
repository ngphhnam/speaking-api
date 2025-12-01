using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class AnalysisResultRepository(ApplicationDbContext context) : IAnalysisResultRepository
{
    public async Task AddAsync(AnalysisResult entity, CancellationToken ct)
        => await context.AnalysisResults.AddAsync(entity, ct);

    public Task<AnalysisResult?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.AnalysisResults
            .Include(ar => ar.Recording)
            .FirstOrDefaultAsync(ar => ar.Id == id, ct);

    public Task<AnalysisResult?> GetByRecordingIdAsync(Guid recordingId, CancellationToken ct)
        => context.AnalysisResults
            .Include(ar => ar.Recording)
            .FirstOrDefaultAsync(ar => ar.RecordingId == recordingId, ct);

    public async Task<IEnumerable<AnalysisResult>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.AnalysisResults
            .AsNoTracking()
            .Include(ar => ar.Recording)
            .Where(ar => ar.UserId == userId)
            .OrderByDescending(ar => ar.AnalyzedAt)
            .ToListAsync(ct);

    public Task UpdateAsync(AnalysisResult entity, CancellationToken ct)
    {
        context.AnalysisResults.Update(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

