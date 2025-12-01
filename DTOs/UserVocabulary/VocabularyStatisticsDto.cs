namespace SpeakingPractice.Api.DTOs.UserVocabulary;

public class VocabularyStatisticsDto
{
    public Guid UserId { get; set; }
    public int TotalVocabulary { get; set; }
    public int LearningCount { get; set; }
    public int ReviewingCount { get; set; }
    public int MasteredCount { get; set; }
    public int DueForReviewCount { get; set; }
    public decimal MasteryPercentage { get; set; }
}

