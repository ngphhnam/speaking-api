namespace SpeakingPractice.Api.DTOs.AnalysisResults;

public class UserStatisticsDto
{
    public Guid UserId { get; set; }
    public int TotalRecordings { get; set; }
    public decimal? AvgOverallScore { get; set; }
    public decimal? AvgFluencyScore { get; set; }
    public decimal? AvgVocabularyScore { get; set; }
    public decimal? AvgGrammarScore { get; set; }
    public decimal? AvgPronunciationScore { get; set; }
    public decimal? HighestScore { get; set; }
    public decimal? LowestScore { get; set; }
    public string[]? WeakAreas { get; set; }
    public string[]? Strengths { get; set; }
}

