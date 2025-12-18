namespace SpeakingPractice.Api.DTOs.Questions;

public class CreateQuestionRequest
{
    public Guid? TopicId { get; set; }
    public string QuestionText { get; set; } = default!;
    public string QuestionType { get; set; } = default!; // PART1, PART2, PART3
    public string? QuestionStyle { get; set; } // OpenEnded, YesNo, CueCard, etc.
    public string? SuggestedStructure { get; set; }
    public string[]? SampleAnswers { get; set; }
    public string[]? KeyVocabulary { get; set; }
    public decimal? EstimatedBandRequirement { get; set; }
    public int TimeLimitSeconds { get; set; } = 120;
}














