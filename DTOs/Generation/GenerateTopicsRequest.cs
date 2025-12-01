namespace SpeakingPractice.Api.DTOs.Generation;

public class GenerateTopicsRequest
{
    public int? PartNumber { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? TopicCategory { get; set; }
    public int Count { get; set; } = 3;
    public string[]? Keywords { get; set; }
}

