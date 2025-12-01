namespace SpeakingPractice.Api.DTOs.UserProgress;

public class ProgressStatisticsDto
{
    public Guid UserId { get; set; }
    public int TotalSessions { get; set; }
    public int TotalRecordings { get; set; }
    public int TotalPracticeMinutes { get; set; }
    public decimal? CurrentAvgScore { get; set; }
    public decimal? BestScore { get; set; }
    public decimal? ImprovementPercentage { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
}

