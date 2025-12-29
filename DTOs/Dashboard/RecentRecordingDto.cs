namespace SpeakingPractice.Api.DTOs.Dashboard;

public class RecentRecordingDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string RecordedAt { get; set; } = string.Empty; // ISO string
    public decimal? OverallScore { get; set; }
}

