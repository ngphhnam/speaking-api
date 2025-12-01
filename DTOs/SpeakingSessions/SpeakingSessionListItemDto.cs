namespace SpeakingPractice.Api.DTOs.SpeakingSessions;

public class SpeakingSessionListItemDto
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = default!;
    public string Level { get; set; } = default!;
    public decimal? BandScore { get; set; }
    public string Status { get; set; } = default!;
    public int? TotalDurationSeconds { get; set; }
    public decimal? OverallBandScore { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

