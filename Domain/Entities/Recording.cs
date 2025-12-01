namespace SpeakingPractice.Api.Domain.Entities;

public class Recording
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid? QuestionId { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public string? AudioFormat { get; set; }
    public long? FileSizeBytes { get; set; }
    public decimal? DurationSeconds { get; set; }
    public string? TranscriptionText { get; set; }
    public decimal? TranscriptionConfidence { get; set; }
    public string TranscriptionLanguage { get; set; } = "en";
    public string? WordTimestamps { get; set; } // JSONB
    public string ProcessingStatus { get; set; } = "pending";
    public string? ErrorMessage { get; set; }
    public string? RefinedText { get; set; } // AI-refined version
    public string? RefinementMetadata { get; set; } // JSONB with improvement details
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual PracticeSession Session { get; set; } = default!;
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual Question? Question { get; set; }
    public virtual AnalysisResult? AnalysisResult { get; set; }
}

