namespace SpeakingPractice.Api.Domain.Entities;

public class UserDraft
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string DraftContent { get; set; } = string.Empty;
    public string? OutlineStructure { get; set; } // JSONB
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual Question Question { get; set; } = default!;
}

