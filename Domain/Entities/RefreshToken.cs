namespace SpeakingPractice.Api.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public bool IsRevoked => RevokedAt.HasValue || DateTimeOffset.UtcNow >= ExpiresAt;
}

