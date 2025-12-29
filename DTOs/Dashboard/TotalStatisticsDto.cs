namespace SpeakingPractice.Api.DTOs.Dashboard;

public class TotalStatisticsDto
{
    public int TotalQuestions { get; set; }
    public int TotalRecordings { get; set; }
    public decimal? AverageScore { get; set; }
    public int TotalPracticeTime { get; set; } // minutes
    public int StreakDays { get; set; }
    public string? LastPracticeDate { get; set; } // ISO string
}

