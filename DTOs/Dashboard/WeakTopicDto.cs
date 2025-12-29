namespace SpeakingPractice.Api.DTOs.Dashboard;

public class WeakTopicDto
{
    public Guid TopicId { get; set; }
    public string TopicTitle { get; set; } = string.Empty;
    public decimal? AverageScore { get; set; }
    public int TotalAttempts { get; set; }
}

