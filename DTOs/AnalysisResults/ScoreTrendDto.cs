namespace SpeakingPractice.Api.DTOs.AnalysisResults;

public class ScoreTrendDto
{
    public DateTimeOffset Date { get; set; }
    public decimal? OverallScore { get; set; }
    public decimal? FluencyScore { get; set; }
    public decimal? VocabularyScore { get; set; }
    public decimal? GrammarScore { get; set; }
    public decimal? PronunciationScore { get; set; }
}

