namespace SpeakingPractice.Api.DTOs.Questions;

public class CreateQuestionRequest
{
    public Guid? TopicId { get; set; }
    public string QuestionText { get; set; } = default!;
    public string? SuggestedStructure { get; set; }
    public string[]? SampleAnswers { get; set; }
    public string[]? KeyVocabulary { get; set; }
}














