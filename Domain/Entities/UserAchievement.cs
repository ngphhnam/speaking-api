namespace SpeakingPractice.Api.Domain.Entities;

public class UserAchievement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public string? Progress { get; set; } // JSONB
    public bool IsCompleted { get; set; } = false;
    public DateTimeOffset? EarnedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual Achievement Achievement { get; set; } = default!;
}

