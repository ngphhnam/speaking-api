namespace SpeakingPractice.Api.DTOs.Achievements;

public class UpdateAchievementRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AchievementType { get; set; }
    public string? RequirementCriteria { get; set; }
    public int? Points { get; set; }
    public string? BadgeIconUrl { get; set; }
    public bool? IsActive { get; set; }
}

