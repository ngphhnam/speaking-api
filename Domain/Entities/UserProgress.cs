namespace SpeakingPractice.Api.Domain.Entities;

public class UserProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string PeriodType { get; set; } = string.Empty; // 'daily', 'weekly', 'monthly'
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    
    // Practice stats
    public int TotalSessions { get; set; } = 0;
    public int TotalRecordings { get; set; } = 0;
    public int TotalPracticeMinutes { get; set; } = 0;
    
    // Score trends
    public decimal? AvgOverallScore { get; set; }
    public decimal? AvgFluencyScore { get; set; }
    public decimal? AvgVocabularyScore { get; set; }
    public decimal? AvgGrammarScore { get; set; }
    public decimal? AvgPronunciationScore { get; set; }
    
    // Improvement metrics
    public decimal? ScoreImprovement { get; set; }
    public decimal? ConsistencyScore { get; set; }
    
    // Detailed breakdown
    public string? TopicsPracticed { get; set; } // JSONB
    public string[]? WeakestAreas { get; set; } // JSONB[]
    public string[]? StrongestAreas { get; set; } // JSONB[]
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = default!;
}

