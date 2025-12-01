namespace SpeakingPractice.Api.Domain.Entities;

public class UserVocabulary
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid VocabularyId { get; set; }
    public string LearningStatus { get; set; } = "learning"; // 'new', 'learning', 'reviewing', 'mastered'
    public DateTimeOffset? NextReviewAt { get; set; }
    public int ReviewCount { get; set; } = 0;
    public int SuccessCount { get; set; } = 0;
    public string? PersonalNotes { get; set; }
    public string? ExampleUsage { get; set; }
    public DateTimeOffset FirstEncounteredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastReviewedAt { get; set; }
    public DateTimeOffset? MasteredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual Vocabulary Vocabulary { get; set; } = default!;
}

