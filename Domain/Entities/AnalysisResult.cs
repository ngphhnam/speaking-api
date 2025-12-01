namespace SpeakingPractice.Api.Domain.Entities;

public class AnalysisResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RecordingId { get; set; }
    public Guid UserId { get; set; }
    public decimal? OverallBandScore { get; set; }
    public decimal? FluencyScore { get; set; }
    public decimal? VocabularyScore { get; set; }
    public decimal? GrammarScore { get; set; }
    public decimal? PronunciationScore { get; set; }
    public string Metrics { get; set; } = "{}"; // JSONB
    public string? FeedbackSummary { get; set; }
    public string[]? Strengths { get; set; } // JSONB[]
    public string[]? Improvements { get; set; } // JSONB[]
    public string[]? GrammarIssues { get; set; } // JSONB[]
    public string[]? PronunciationIssues { get; set; } // JSONB[]
    public string[]? VocabularySuggestions { get; set; } // JSONB[]
    public string? WhisperModelVersion { get; set; }
    public string? LlamaModelVersion { get; set; }
    public string? AnalysisEngineVersion { get; set; }
    public string? RefinementSuggestions { get; set; } // JSONB with specific suggestions
    public string? ComparisonAnalysis { get; set; } // JSONB comparing original vs refined
    public DateTimeOffset AnalyzedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual Recording Recording { get; set; } = default!;
    public virtual ApplicationUser User { get; set; } = default!;
}

