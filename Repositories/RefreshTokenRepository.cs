using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token, CancellationToken ct)
        => await context.RefreshTokens.AddAsync(token, ct);

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
        => context.RefreshTokens.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public Task RevokeAsync(RefreshToken token, CancellationToken ct)
    {
        token.RevokedAt = DateTimeOffset.UtcNow;
        context.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

