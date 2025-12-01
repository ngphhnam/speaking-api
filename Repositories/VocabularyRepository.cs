using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class VocabularyRepository(ApplicationDbContext context) : IVocabularyRepository
{
    public async Task AddAsync(Vocabulary entity, CancellationToken ct)
        => await context.Vocabularies.AddAsync(entity, ct);

    public Task<Vocabulary?> GetByIdAsync(Guid id, CancellationToken ct)
        => context.Vocabularies
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public Task<Vocabulary?> GetByWordAsync(string word, CancellationToken ct)
        => context.Vocabularies
            .FirstOrDefaultAsync(v => v.Word.ToLower() == word.ToLower(), ct);

    public async Task<IEnumerable<Vocabulary>> GetAllAsync(CancellationToken ct)
        => await context.Vocabularies
            .AsNoTracking()
            .OrderBy(v => v.Word)
            .ToListAsync(ct);

    public async Task<IEnumerable<Vocabulary>> SearchAsync(string query, CancellationToken ct)
        => await context.Vocabularies
            .AsNoTracking()
            .Where(v => v.Word.Contains(query, StringComparison.OrdinalIgnoreCase)
                || (v.DefinitionEn != null && v.DefinitionEn.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(v => v.Word)
            .ToListAsync(ct);

    public async Task<IEnumerable<Vocabulary>> GetByBandLevelAsync(decimal? minBand, decimal? maxBand, CancellationToken ct)
    {
        var query = context.Vocabularies.AsNoTracking();
        
        if (minBand.HasValue)
            query = query.Where(v => v.IeltsBandLevel >= minBand.Value);
        
        if (maxBand.HasValue)
            query = query.Where(v => v.IeltsBandLevel <= maxBand.Value);
        
        return await query.OrderBy(v => v.Word).ToListAsync(ct);
    }

    public async Task<IEnumerable<Vocabulary>> GetByTopicCategoryAsync(string category, CancellationToken ct)
        => await context.Vocabularies
            .AsNoTracking()
            .Where(v => v.TopicCategories != null && v.TopicCategories.Contains(category))
            .OrderBy(v => v.Word)
            .ToListAsync(ct);

    public Task UpdateAsync(Vocabulary entity, CancellationToken ct)
    {
        context.Vocabularies.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Vocabulary entity, CancellationToken ct)
    {
        context.Vocabularies.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

