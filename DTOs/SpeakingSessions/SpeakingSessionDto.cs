namespace SpeakingPractice.Api.DTOs.SpeakingSessions;

public class SpeakingSessionDto
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = default!;
    public string Level { get; set; } = default!;
    public string AudioUrl { get; set; } = default!;
    public string? TranscriptionText { get; set; }
    public decimal? BandScore { get; set; }
    public decimal? PronunciationScore { get; set; }
    public decimal? GrammarScore { get; set; }
    public decimal? VocabularyScore { get; set; }
    public decimal? FluencyScore { get; set; }
    public string? OverallFeedback { get; set; }
    public string? GrammarReportJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

