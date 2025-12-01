namespace SpeakingPractice.Api.Domain.Entities;

public class Topic
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PartNumber { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? TopicCategory { get; set; }
    public string[]? Keywords { get; set; }
    public int UsageCount { get; set; } = 0;
    public decimal? AvgUserRating { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
}

