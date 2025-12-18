namespace SpeakingPractice.Api.DTOs.Topics;

public class TopicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public int? PartNumber { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? TopicCategory { get; set; }
    public string[]? Keywords { get; set; }
    public int UsageCount { get; set; }
    public decimal? AvgUserRating { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int TotalQuestion { get; set; }
    public int Part1QuestionCount { get; set; }
    public int Part2QuestionCount { get; set; }
    public int Part3QuestionCount { get; set; }
}














