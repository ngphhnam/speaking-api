namespace SpeakingPractice.Api.DTOs.Drafts;

public class UserDraftDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string DraftContent { get; set; } = string.Empty;
    public string? OutlineStructure { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

