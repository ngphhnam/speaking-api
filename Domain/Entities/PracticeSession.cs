namespace SpeakingPractice.Api.Domain.Entities;

public class PracticeSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string? SessionType { get; set; }
    public int? PartNumber { get; set; }
    public Guid? TopicId { get; set; }
    public int QuestionsAttempted { get; set; } = 0;
    public int? TotalDurationSeconds { get; set; }
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public string Status { get; set; } = "in_progress";
    public decimal? OverallBandScore { get; set; }
    public decimal? FluencyScore { get; set; }
    public decimal? VocabularyScore { get; set; }
    public decimal? GrammarScore { get; set; }
    public decimal? PronunciationScore { get; set; }
    public string? DeviceInfo { get; set; } // JSONB
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual Topic? Topic { get; set; }
    public virtual ICollection<Recording> Recordings { get; set; } = new List<Recording>();
}

