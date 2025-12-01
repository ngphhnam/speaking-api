namespace SpeakingPractice.Api.DTOs.Topics;

public class UpdateTopicRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? PartNumber { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? TopicCategory { get; set; }
    public string[]? Keywords { get; set; }
    public bool? IsActive { get; set; }
}














