namespace SpeakingPractice.Api.DTOs.UserProgress;

public class UserProgressDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PeriodType { get; set; } = string.Empty;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public int TotalSessions { get; set; }
    public int TotalRecordings { get; set; }
    public int TotalPracticeMinutes { get; set; }
    public decimal? AvgOverallScore { get; set; }
    public decimal? AvgFluencyScore { get; set; }
    public decimal? AvgVocabularyScore { get; set; }
    public decimal? AvgGrammarScore { get; set; }
    public decimal? AvgPronunciationScore { get; set; }
    public decimal? ScoreImprovement { get; set; }
    public decimal? ConsistencyScore { get; set; }
    public string[]? WeakestAreas { get; set; }
    public string[]? StrongestAreas { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

