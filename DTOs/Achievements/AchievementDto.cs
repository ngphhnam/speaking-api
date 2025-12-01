namespace SpeakingPractice.Api.DTOs.Achievements;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AchievementType { get; set; } = string.Empty;
    public int Points { get; set; }
    public string? BadgeIconUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

