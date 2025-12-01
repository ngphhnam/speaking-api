using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
    Task RevokeAsync(RefreshToken token, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

