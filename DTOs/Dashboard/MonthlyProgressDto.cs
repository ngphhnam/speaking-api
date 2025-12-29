namespace SpeakingPractice.Api.DTOs.Dashboard;

public class MonthlyProgressDto
{
    public string Month { get; set; } = string.Empty; // e.g., "thg 12, 2024"
    public decimal? AverageScore { get; set; }
    public int TotalRecordings { get; set; }
}

