namespace SpeakingPractice.Api.DTOs.Generation;

public class GenerateQuestionsRequest
{
    public Guid TopicId { get; set; }
    public int Count { get; set; } = 5;
}

