namespace SpeakingPractice.Api.DTOs.Achievements;

public class CreateAchievementRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string AchievementType { get; set; } = string.Empty;
    public string RequirementCriteria { get; set; } = "{}";
    public int Points { get; set; }
    public string? BadgeIconUrl { get; set; }
}

