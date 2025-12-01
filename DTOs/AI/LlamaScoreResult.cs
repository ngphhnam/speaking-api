namespace SpeakingPractice.Api.DTOs.AI;

public class LlamaScoreResult
{
    public decimal BandScore { get; set; }
    public decimal PronunciationScore { get; set; }
    public decimal GrammarScore { get; set; }
    public decimal VocabularyScore { get; set; }
    public decimal FluencyScore { get; set; }
    public string OverallFeedback { get; set; } = string.Empty;
}

