namespace SpeakingPractice.Api.DTOs.Generation;

public class GenerateQuestionsRequest
{
    public Guid TopicId { get; set; }
    public int Count { get; set; } = 5;
    public string? QuestionType { get; set; }
    public decimal? EstimatedBandRequirement { get; set; }
}

