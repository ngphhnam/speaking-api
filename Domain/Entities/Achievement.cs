namespace SpeakingPractice.Api.Domain.Entities;

public class Achievement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AchievementType { get; set; } = string.Empty; // 'practice_streak', 'score_milestone', etc.
    public string RequirementCriteria { get; set; } = "{}"; // JSONB
    public int Points { get; set; } = 0;
    public string? BadgeIconUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}

