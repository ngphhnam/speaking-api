namespace SpeakingPractice.Api.DTOs.Recordings;

public class RecordingDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid? QuestionId { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public string? AudioFormat { get; set; }
    public long? FileSizeBytes { get; set; }
    public decimal? DurationSeconds { get; set; }
    public string? TranscriptionText { get; set; }
    public string TranscriptionLanguage { get; set; } = "en";
    public string ProcessingStatus { get; set; } = "pending";
    public string? RefinedText { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

