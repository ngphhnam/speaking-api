namespace SpeakingPractice.Api.DTOs.Questions;

public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid? TopicId { get; set; }
    public string? TopicTitle { get; set; }
    public string QuestionText { get; set; } = default!;
    public string? QuestionType { get; set; }
    public string? SuggestedStructure { get; set; }
    public string[]? SampleAnswers { get; set; }
    public string[]? KeyVocabulary { get; set; }
    public decimal? EstimatedBandRequirement { get; set; }
    public int TimeLimitSeconds { get; set; }
    public int AttemptsCount { get; set; }
    public decimal? AvgScore { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}














