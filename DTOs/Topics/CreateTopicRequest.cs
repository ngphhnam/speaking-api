namespace SpeakingPractice.Api.DTOs.Topics;

public class CreateTopicRequest
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int? PartNumber { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? TopicCategory { get; set; }
    public string[]? Keywords { get; set; }
}














