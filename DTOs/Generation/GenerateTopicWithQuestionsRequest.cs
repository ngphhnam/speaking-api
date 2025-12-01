namespace SpeakingPractice.Api.DTOs.Generation;

public class GenerateTopicWithQuestionsRequest
{
    public string TopicTitle { get; set; } = default!;
    public int? PartNumber { get; set; }
    public string? DifficultyLevel { get; set; }
    public int QuestionCount { get; set; } = 3;
}

