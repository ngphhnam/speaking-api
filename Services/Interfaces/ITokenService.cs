using System.Security.Claims;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface ITokenService
{
    Task<(string Token, DateTimeOffset ExpiresAt)> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken ct);
    Task<IReadOnlyCollection<string>> GetUserRolesAsync(ApplicationUser user);
    RefreshToken GenerateRefreshToken(Guid userId);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

