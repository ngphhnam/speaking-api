namespace SpeakingPractice.Api.Domain.Entities;

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TopicId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionType { get; set; }
    public string? SuggestedStructure { get; set; } // JSONB
    public string[]? SampleAnswers { get; set; } // JSONB[]
    public string[]? KeyVocabulary { get; set; } // JSONB[]
    public decimal? EstimatedBandRequirement { get; set; }
    public int TimeLimitSeconds { get; set; } = 120;
    public int AttemptsCount { get; set; } = 0;
    public decimal? AvgScore { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual Topic? Topic { get; set; }
    public virtual ICollection<Recording> Recordings { get; set; } = new List<Recording>();
}

