namespace SpeakingPractice.Api.DTOs.Common;

public class StatisticsDto
{
    public int TotalUsers { get; set; }
    public int TotalSessions { get; set; }
    public int TotalRecordings { get; set; }
    public int TotalTopics { get; set; }
    public int TotalQuestions { get; set; }
    public decimal? AvgOverallScore { get; set; }
}

