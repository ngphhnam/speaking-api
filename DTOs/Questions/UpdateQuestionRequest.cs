namespace SpeakingPractice.Api.DTOs.Questions;

public class UpdateQuestionRequest
{
    public Guid? TopicId { get; set; }
    public string? QuestionText { get; set; }
    public string? QuestionType { get; set; }
    public string? SuggestedStructure { get; set; }
    public string[]? SampleAnswers { get; set; }
    public string[]? KeyVocabulary { get; set; }
    public decimal? EstimatedBandRequirement { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public bool? IsActive { get; set; }
}














