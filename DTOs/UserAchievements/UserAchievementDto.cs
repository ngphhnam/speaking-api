using SpeakingPractice.Api.DTOs.Achievements;

namespace SpeakingPractice.Api.DTOs.UserAchievements;

public class UserAchievementDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AchievementId { get; set; }
    public AchievementDto Achievement { get; set; } = null!;
    public string? Progress { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset? EarnedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

