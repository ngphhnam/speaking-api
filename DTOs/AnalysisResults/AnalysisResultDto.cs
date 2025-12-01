namespace SpeakingPractice.Api.DTOs.AnalysisResults;

public class AnalysisResultDto
{
    public Guid Id { get; set; }
    public Guid RecordingId { get; set; }
    public Guid UserId { get; set; }
    public decimal? OverallBandScore { get; set; }
    public decimal? FluencyScore { get; set; }
    public decimal? VocabularyScore { get; set; }
    public decimal? GrammarScore { get; set; }
    public decimal? PronunciationScore { get; set; }
    public string? FeedbackSummary { get; set; }
    public string[]? Strengths { get; set; }
    public string[]? Improvements { get; set; }
    public string[]? GrammarIssues { get; set; }
    public string[]? PronunciationIssues { get; set; }
    public string[]? VocabularySuggestions { get; set; }
    public DateTimeOffset AnalyzedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

